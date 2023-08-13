using System;

namespace Twileloop.FileStorage
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string message) : base(message) { }
    }
    public class UnsupportedFileException : Exception
    {
        public UnsupportedFileException(string message) : base(message) { }
    }
    public class EncryptionProviderException : Exception
    {
        public EncryptionProviderException(string message) : base(message) { }
    }
    public class LegacyFormatException : Exception
    {
        public LegacyFormatException(string message) : base(message) { }
    }
}