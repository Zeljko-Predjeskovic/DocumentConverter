using System;

namespace DocumentConverter.Exceptions
{

    public class ScannedPdfException : DocumentConverterException
    {
        public ScannedPdfException(string message) : base(message)
        {
        }

        public ScannedPdfException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}