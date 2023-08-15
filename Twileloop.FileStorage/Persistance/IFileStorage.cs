using System.Collections.Generic;
using Twileloop.FileStorage.Abstractions;

namespace Twileloop.FileStorage.Persistance
{
    public interface IFileStorage<T>
    {
        FileReadResult TryReadFile(string filePath, out FileReadResult fileReadResult, IEncryptionProvider encryptionProvider = null);
        bool WriteFile(T state, string filePath, List<EmbeddedFile> embeddedFiles = null,Dictionary<string, string> meta = null, IEncryptionProvider encryptionProvider = null);
    }
}
