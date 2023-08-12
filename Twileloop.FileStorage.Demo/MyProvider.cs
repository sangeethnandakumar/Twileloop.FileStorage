using Twileloop.FileStorage.Abstractions;

namespace Twileloop.FileStorage.Demo
{
    public class MyProvider : IEncryptionProvider
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

        public MyProvider(string key, string iv)
        {
            this.key = key;
            this.iv = iv;
        }

        public byte[] Encrypt(byte[] rawData)
        {
            return rawData;
        }

        public byte[] Decrypt(byte[] encrypteData)
        {
            return encrypteData;
        }

    }
}
