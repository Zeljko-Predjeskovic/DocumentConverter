using System;

namespace DocumentConverter.Exceptions
{
    public class DocumentConverterException : Exception
    {
        public DocumentConverterException(string message) : base(message)
        {
        }

        public DocumentConverterException(string message, Exception innerException) : base(message, innerException){}
    }
} 