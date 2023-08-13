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
        public FileReadResult ReadFile(string filePath, out FileReadResult fileReadResult, IEncryptionProvider encryptionProvider = null)
        {
            fileReadResult = new FileReadResult();
            try
            {
                //Step 1: Read from file
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    //Step 2: Decompress packet
                    var decompressedBytes = DeflateHelper.DecompressData(buffer);
                    //Step 3: Deserialize data file
                    var xml = Encoding.UTF8.GetString(decompressedBytes);
                    var dataFile = XmlHelper.Deserialize<DataFile>(xml);
                    //Step 4: Make reader output
                    fileReadResult.IsReadSuccessfull = true;
                    fileReadResult.Header = dataFile;
                    fileReadResult.Properties = GetFileProperties(filePath);
                    //Step 5: Decrypt optionaly
                    if (encryptionProvider is not null)
                    {
                        var encryptedData = Convert.FromBase64String(dataFile.EncodedData);
                        dataFile.EncodedData = Convert.ToBase64String(encryptionProvider.Decrypt(encryptedData));
                    }
                }
            }
            catch (Exception ex)
            {
                fileReadResult.IsReadSuccessfull = false;
                fileReadResult.ReaderException = ex;
            }
            return fileReadResult;
        }

        public bool WriteFile(T data, string filePath, IEncryptionProvider encryptionProvider = null)
        {
            try
            {
                //Step 1: Convert data to binary
                var dataBytes = ConvertDataToBytes(data);
                //Step 2: Encrypt optionaly
                if (encryptionProvider is not null)
                {
                    dataBytes = encryptionProvider.Encrypt(dataBytes);
                }
                //Step 3: Make data file
                var dataFileBytes = BuildDataFile(dataBytes);
                //Step 4: Compress packet
                var compresedBytes = DeflateHelper.CompressData(dataFileBytes, CompressionLevel.Optimal);
                //Step 5: Write to file
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

        //Step 1: Convert model to binary
        private byte[] ConvertDataToBytes(T data)
        {
            //Step 1: Serialize to XML
            var xml = XmlHelper.Serialize(data);
            //Step 2: Convert to bytes
            var xmlBytes = Encoding.UTF8.GetBytes(xml);
            return xmlBytes;
        }

        //Step 2: Build a package file
        private byte[] BuildDataFile(byte[] data)
        {
            //Step 1: Serialize to XML
            var fileHeader = XmlHelper.Serialize(new DataFile
            {
                EncodedData = Convert.ToBase64String(data),
                Program = Assembly.GetCallingAssembly().GetName().Name,
                IsEncrypted = false,
                EncryptionAlgorithm = "Not Available",
                FileMeta = new(),
                DataSize = data.Length
            });
            return Encoding.UTF8.GetBytes(fileHeader);
        }
    }
}
