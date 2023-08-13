namespace Twileloop.FileStorage.Abstractions
{
    public interface IEncryptionProvider
    {
        string GetEncryptionProvider();
        string GetEncryptionAlgorithm();
        byte[] Encrypt(byte[] rawData);
        byte[] Decrypt(byte[] encrypteData);
    }
}
