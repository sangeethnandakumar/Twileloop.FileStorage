namespace Twileloop.FileStorage.Abstractions
{
    public interface IEncryptionProvider
    {
        string GetEncryptionProvider();
        string GetEncryptionAlgorithm();
        Credential SetKeyAndInitialVector();
        byte[] Encrypt(byte[] rawData);
        byte[] Decrypt(byte[] encrypteData);
    }
}
