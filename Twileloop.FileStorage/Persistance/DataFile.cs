using System.Collections.Generic;

namespace Twileloop.FileStorage.Persistance
{
    public class DataFile
    {
        public string Program { get; set; }
        public string EncodedData { get; set; }
        public bool IsEncrypted { get; set; }
        public string EncryptionAlgorithm { get; set; }
        public long DataSize { get; set; }
        public Dictionary<string, string> FileMeta { get; set; }
    }
}
