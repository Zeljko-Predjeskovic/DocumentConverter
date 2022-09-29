using System;

namespace DocumentConverter.Plugin.Shared
{
    public class DocumentConverterException : Exception
    {
        public DocumentConverterException(string message) : base(message)
        {
        }

        public DocumentConverterException(string message, Exception innerException) : base(message, innerException){}
    }
} 