using System;
using System.Collections.Generic;
using System.IO;

namespace Twileloop.FileStorage.Abstractions
{
    public class EmbeddedFile
    {
        public string Key { get; set; }
        public string Data { get; set; }
    }

    public class EmbeddedFileBuilder
    {
        private readonly List<EmbeddedFile> _embeddedFiles;

        public EmbeddedFileBuilder()
        {
            _embeddedFiles = new();
        }

        public EmbeddedFileBuilder AddFile(string key, byte[] data)
        {
            var _embeddedFile = new EmbeddedFile();
            _embeddedFile.Key = key;
            _embeddedFile.Data = Convert.ToBase64String(data);
            _embeddedFiles.Add(_embeddedFile);
            return this;
        }

        public EmbeddedFileBuilder AddFile(string key, string path)
        {
            var _embeddedFile = new EmbeddedFile();
            _embeddedFile.Key = key;
            _embeddedFile.Data = ReadBinaryFile(path);
            _embeddedFiles.Add(_embeddedFile);
            return this;
        }

        public List<EmbeddedFile> Build()
        {
            return _embeddedFiles;
        }

        static string ReadBinaryFile(string filePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        long fileLength = new FileInfo(filePath).Length;
                        byte[] fileData = binaryReader.ReadBytes((int)fileLength);
                        return Convert.ToBase64String(fileData);
                    }
                }
            }
            catch (IOException)
            {
                throw new FileNotFoundException("Please ensure the embedded file locations are correct");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class MetaDataBuilder
    {
        private readonly Dictionary<string, string> _metaData;

        public MetaDataBuilder()
        {
            _metaData = new();
        }

        public MetaDataBuilder AddMeta(string key, string value)
        {
            _metaData.Add(key, value);
            return this;
        }

        public Dictionary<string, string> Build()
        {
            return _metaData;
        }
    }
}
