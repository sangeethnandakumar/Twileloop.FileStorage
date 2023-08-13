using Twileloop.FileStorage.Abstractions;
using Twileloop.Security.Encryption;

namespace Twileloop.FileStorage.Demo
{
    public class MyCustomSecurityProvider : IEncryptionProvider
    {
        private readonly string key;
        private readonly string iv;

        public string GetEncryptionProvider() => "Twileloop.Security";
        public string GetEncryptionAlgorithm() => "AES";
        public Credential SetKeyAndInitialVector() => new Credential
        {
            Key = key,
            IV = iv
        };

        public MyCustomSecurityProvider(string key, string iv)
        {
            this.key = key;
            this.iv = iv;
        }

        public byte[] Encrypt(byte[] rawData)
        {
            //return rawData;
            return AESAlgorithm.EncryptBytes(rawData, key, iv);
        }

        public byte[] Decrypt(byte[] encrypteData)
        {
            //return encrypteData;
            return AESAlgorithm.DecryptBytes(encrypteData, key, iv);
        }

    }
}
