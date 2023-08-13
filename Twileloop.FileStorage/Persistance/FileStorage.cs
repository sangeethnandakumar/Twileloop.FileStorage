using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Twileloop.FileStorage.Abstractions;
using Twileloop.FileStorage.Engines;
using Twileloop.FileStorage.Persistance.Internal;

namespace Twileloop.FileStorage.Persistance
{

    public class FileStorage<T> : IFileStorage<T>
    {
        public FileReadResult TryReadFile(string filePath, out FileReadResult fileReadResult, IEncryptionProvider encryptionProvider = null)
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
                    if(!string.IsNullOrEmpty(dataFile.FileFormat) && dataFile.FileFormat != "PDR.v1")
                    {
                        throw new LegacyFormatException($"Reading aborted. The content of this file is not in 'PDR.v1' (Portable Data Recording) file format. This file is using '{dataFile.FileFormat}' format which was written using '{dataFile.UsedVersion}' version of Twileloop.FileStorage library. To read this file, upgrade the library to version '{dataFile.UsedVersion}+' or above version");
                    }
                    //Step 4: Make reader output
                    fileReadResult.IsReadSuccessfull = true;
                    fileReadResult.Header = dataFile;
                    fileReadResult.Properties = GetFileProperties(filePath);
                    //Step 5: Decrypt optionaly
                    if (dataFile.IsEncrypted)
                    {
                        if (encryptionProvider is null)
                        {
                            fileReadResult.IsReadSuccessfull = false;
                            throw new EncryptionProviderException($"This file is encrypted using '{dataFile.EncryptionAlgorithm}' algorithm. Please provide an 'EncryptionProvider' that can decrypt the contents.");
                        }
                        else
                        {
                            var encryptedData = Convert.FromBase64String(dataFile.EncodedData);
                            dataFile.EncodedData = Convert.ToBase64String(encryptionProvider.Decrypt(encryptedData));
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                fileReadResult.IsReadSuccessfull = false;
                throw new InvalidPasswordException("Decryption failed. Please ensure the algorithms and credentials used for encryption/decryption are valid");
            }
            catch (InvalidDataException)
            {
                fileReadResult.IsReadSuccessfull = false;
               throw new UnsupportedFileException("Decryption failed. Please ensure the algorithms and credentials used for encryption/decryption are valid");
            }
            catch (Exception)
            {
                fileReadResult.IsReadSuccessfull = false;
                throw;
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
                var dataFileBytes = BuildDataFile(dataBytes, encryptionProvider);
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
        private byte[] BuildDataFile(byte[] data, IEncryptionProvider provider)
        {
            //Step 1: Serialize to XML
            var fileHeader = XmlHelper.Serialize(new DataFile
            {
                EncodedData = Convert.ToBase64String(data),
                Program = Assembly.GetCallingAssembly().FullName,
                IsEncrypted = provider is not null,
                EncryptionAlgorithm = provider is not null? provider.GetEncryptionAlgorithm() : "Not Available",
                EncryptionProvider = provider is not null? provider.GetEncryptionProvider() : "Not Available",
                FileMeta = new(),
                DataSize = data.Length
            });
            return Encoding.UTF8.GetBytes(fileHeader);
        }
    }
}
