using Twileloop.FileStorage.Abstractions;
using Twileloop.FileStorage.Persistance.Internal;

namespace Twileloop.FileStorage.Persistance
{
    public interface IFileStorage<T>
    {
        FileReadResult<T> ReadFile(string filePath, out FileReadResult<T> fileReadResult, IEncryptionProvider encryptionProvider = null);
        bool WriteFile(T state, string filePath, IEncryptionProvider encryptionProvider = null);
    }
}
