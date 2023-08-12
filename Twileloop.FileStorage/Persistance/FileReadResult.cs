using System;
using Twileloop.FileStorage.Persistance.Internal;

namespace Twileloop.FileStorage.Persistance
{
    public class FileReadResult<T>
    {
        public bool IsReadSuccessfull { get; set; }
        public Exception ReaderException { get; set; }
        public T Data { get; set; }
        public FileHeader Header { get; set; }
        public FileProperties Properties { get; set; }

        public static implicit operator bool(FileReadResult<T> result)
        {
            return result.IsReadSuccessfull;
        }
    }
}
