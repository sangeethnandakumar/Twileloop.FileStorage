using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Twileloop.FileStorage.Abstractions;
using Twileloop.FileStorage.Engines;
using Twileloop.FileStorage.Persistance.Internal;

namespace Twileloop.FileStorage.Persistance
{

    public class FileStorage<T> : IFileStorage<T>
    {
        public FileReadResult<T> ReadFile(string filePath, out FileReadResult<T> fileReadResult, IEncryptionProvider encryptionProvider = null)
        {
            fileReadResult = new FileReadResult<T>();
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    var decompressedBytes = DeflateHelper.DecompressData(buffer);
                    //Step 1: Deserialize data file
                    var xml = Encoding.UTF8.GetString(decompressedBytes);
                    var headerInfo = XmlHelper.Deserialize<FileHeader>(xml);
                    //Step 2: Decrypt if required
                    var serializedXmlData = "";
                    if (headerInfo.IsEncrypted)
                    {
                        if (encryptionProvider is null)
                        {
                            fileReadResult.IsReadSuccessfull = false;
                            fileReadResult.ReaderException = new Exception($"This file is encrypted using '{headerInfo.EncryptionAlgorithm}' algorithm. Please provide an 'EncryptionProvider' that can decrypt the contents.");
                            return fileReadResult;
                        }
                        //Try decrypting
                        try
                        {
                            var decryptedBytes = encryptionProvider.Decrypt(Encoding.UTF8.GetBytes(headerInfo.Data));
                            var data = Encoding.UTF8.GetString(decryptedBytes);
                            var dataBytes = Convert.FromBase64String(data);
                            serializedXmlData = Encoding.UTF8.GetString(dataBytes);
                        }
                        catch
                        {
                            fileReadResult.IsReadSuccessfull = false;
                            fileReadResult.ReaderException = new Exception($"Decryption failed. Please ensure the 'EncryptionProvider' is using correct credentials to decrypt this file.");
                            return fileReadResult;
                        }
                    }
                    else
                    {
                        var dataBytes = Convert.FromBase64String(headerInfo.Data);
                        serializedXmlData = Encoding.UTF8.GetString(dataBytes);
                    }
                    //Step 2: Inspect meta
                    fileReadResult.Header = headerInfo;
                    fileReadResult.Properties = GetFileProperties(filePath);
                    fileReadResult.IsReadSuccessfull = true;
                    fileReadResult.ReaderException = null;
                    fileReadResult.Data = XmlHelper.Deserialize<T>(serializedXmlData);
                    return fileReadResult;
                }
            }
            catch (Exception ex)
            {
                fileReadResult.IsReadSuccessfull = false;
                fileReadResult.ReaderException = ex;
            }
            return fileReadResult;
        }

        public bool WriteFile(T state, string filePath, IEncryptionProvider encryptionProvider = null)
        {
            try
            {
                //Step 1: Serialize data
                var xml = XmlHelper.Serialize(state);
                //Step 2: Encrypt
                var dataBytes = Encoding.UTF8.GetBytes(xml);
                if (encryptionProvider is not null)
                {
                    dataBytes = encryptionProvider.Encrypt(dataBytes);
                }
                //Step 4: Package
                var dataFile = new FileHeader
                {
                    Program = Assembly.GetCallingAssembly().GetName().Name,
                    Data = Convert.ToBase64String(dataBytes),
                    IsEncrypted = encryptionProvider is not null,
                    EncryptionAlgorithm = encryptionProvider is not null ? encryptionProvider.GetEncryptionAlgorithm() : "Not Available",
                    FileMeta = new(),
                    DataSize = dataBytes.Length
                };
                //Step 5: Serialize package
                var packetXml = XmlHelper.Serialize(dataFile);
                //Step 3: Compress packet
                var packetBytes = Encoding.UTF8.GetBytes(packetXml);
                var compresedBytes = DeflateHelper.CompressData(packetBytes, CompressionLevel.Optimal);
                //Step 6: Write to file
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    fileStream.Write(compresedBytes, 0, compresedBytes.Length);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private FileProperties GetFileProperties(string fileLocation)
        {
            try
            {
                var fileDetails = new FileProperties();
                var fileInfo = new FileInfo(fileLocation);
                fileDetails.FileName = Path.GetFileName(fileLocation);
                fileDetails.FileLocation = Path.GetFullPath(fileLocation);
                fileDetails.Extension = Path.GetExtension(fileLocation);
                fileDetails.FileSizeBytes = fileInfo.Length;
                fileDetails.CreatedDate = fileInfo.CreationTime;
                fileDetails.LastModifiedDate = fileInfo.LastWriteTime;
                return fileDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
