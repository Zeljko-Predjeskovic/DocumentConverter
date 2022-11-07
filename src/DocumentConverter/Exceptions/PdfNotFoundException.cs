using System;

namespace DocumentConverter.Exceptions
{

    public class PdfNotFoundException : DocumentConverterException
    {
        public PdfNotFoundException(string message) : base(message)
        {
        }

        public PdfNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}