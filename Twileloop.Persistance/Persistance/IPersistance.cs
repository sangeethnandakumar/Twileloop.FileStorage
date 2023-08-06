using System.Threading.Tasks;
using Twileloop.SessionGuard.Persistance.Internal;

namespace Twileloop.SessionGuard.Persistance
{
    public interface IPersistance<T>
    {
        bool ReadFile(string filePath, out FileDetails<T> fileDetails);
        bool WriteFile(T state, string filePath);
    }
}
