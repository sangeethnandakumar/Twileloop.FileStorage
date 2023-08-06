using Twileloop.FileStorage.Persistance.Internal;

namespace Twileloop.FileStorage.Persistance
{
    public interface IPersistance<T>
    {
        bool ReadFile(string filePath, out FileDetails<T> fileDetails);
        bool WriteFile(T state, string filePath);
    }
}
