using Twileloop.FileStorage.Abstractions;

namespace Twileloop.FileStorage.Persistance
{
    public interface IFileStorage<T>
    {
        FileReadResult ReadFile(string filePath, out FileReadResult fileReadResult, IEncryptionProvider encryptionProvider = null);
        bool WriteFile(T state, string filePath, IEncryptionProvider encryptionProvider = null);
    }
}
