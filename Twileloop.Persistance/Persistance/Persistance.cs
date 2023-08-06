using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Twileloop.SessionGuard.Engines;
using Twileloop.SessionGuard.Exceptions;
using Twileloop.SessionGuard.Persistance.Internal;

namespace Twileloop.SessionGuard.Persistance
{

    public class Persistance<T> : IPersistance<T>
    {
        public async Task<FileDetails<T>> ReadFileAsync(string filePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    byte[] buffer = new byte[fileStream.Length];
                    await fileStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    var decompressedBytes = DeflateHelper.DecompressData(buffer);
                    var xml = Encoding.UTF8.GetString(decompressedBytes);
                    var data = XmlHelper.Deserialize<T>(xml);
                    var fileDetails = GetFileDetails(filePath, data);
                    return fileDetails;
                }
            }
            catch (Exception ex)
            {
                throw new FileAccessException(ex, filePath, true);
            }
        }

        public async Task WriteFileAsync(T state, string filePath)
        {
            try
            {
                var xml = XmlHelper.Serialize(state);
                var compresedBytes = DeflateHelper.CompressData(Encoding.UTF8.GetBytes(xml), CompressionLevel.Optimal);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await fileStream.WriteAsync(compresedBytes, 0, compresedBytes.Length).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new FileAccessException(ex, filePath, false);
            }
        }

        private FileDetails<T> GetFileDetails<T>(string fileLocation, T data)
        {
            try
            {
                var fileDetails = new FileDetails<T>();
                FileInfo fileInfo = new FileInfo(fileLocation);

                fileDetails.FileName = Path.GetFileName(fileLocation);
                fileDetails.FileLocation = Path.GetFullPath(fileLocation);
                fileDetails.Extension = Path.GetExtension(fileLocation);
                fileDetails.FileSizeBytes = fileInfo.Length;
                fileDetails.Data = data;
                fileDetails.CreatedDate = fileInfo.CreationTime;
                fileDetails.LastModifiedDate = fileInfo.LastWriteTime;

                return fileDetails;
            }
            catch (Exception ex)
            {
                // Handle exceptions here (you can log, throw, or handle it as required)
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // or throw an exception if needed
            }
        }
    }
}
