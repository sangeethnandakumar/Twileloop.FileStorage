using System;
using System.Text;
using Twileloop.FileStorage.Engines;
using Twileloop.FileStorage.Persistance.Internal;

namespace Twileloop.FileStorage.Persistance
{
    public class FileReadResult
    {
        public bool IsReadSuccessfull { get; set; }
        public DataFile Header { get; set; }
        public FileProperties Properties { get; set; }

        public T ParseContents<T>()
        {
            try
            {
                return XmlHelper.Deserialize<T>(Encoding.UTF8.GetString(Convert.FromBase64String(Header.EncodedData)));
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"The file contents are not parsable to '{nameof(T)}'. If you need to parse explicitly, Access the 'Data' as bytes explicitly and process, instead of 'Read<{nameof(T)}>()' funtion");
            }
        }

        public static implicit operator bool(FileReadResult result)
        {
            return result.IsReadSuccessfull;
        }
    }
}
