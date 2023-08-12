using System;

namespace Twileloop.FileStorage.Persistance.Internal
{
    public class FileProperties
    {
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public string Extension { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
