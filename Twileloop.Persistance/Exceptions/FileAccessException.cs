using System;

namespace Twileloop.SessionGuard.Exceptions
{
    public class FileAccessException : Exception
    {
        public FileAccessException(Exception baseException, string fileLocation, bool isRead)
        {
            BaseException = baseException;
            FileLocation = fileLocation;
            IsRead = isRead;
        }

        public Exception BaseException { get; set; }
        public string FileLocation { get; set; }
        public bool IsRead { get; set; }
    }
}
