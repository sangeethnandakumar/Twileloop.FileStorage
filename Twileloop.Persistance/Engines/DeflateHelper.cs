using System.IO.Compression;
using System.IO;

namespace Twileloop.SessionGuard.Engines
{
    public static class DeflateHelper
    {
        public static byte[] CompressData(byte[] data, CompressionLevel compressionLevel)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (DeflateStream compressionStream = new DeflateStream(outputStream, compressionLevel))
                {
                    compressionStream.Write(data, 0, data.Length);
                }
                return outputStream.ToArray();
            }
        }

        public static byte[] DecompressData(byte[] compressedData)
        {
            using (MemoryStream inputStream = new MemoryStream(compressedData))
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (DeflateStream decompressionStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(outputStream);
                }
                return outputStream.ToArray();
            }
        }
    }
}
