using System;

namespace DocumentConverter.Exceptions
{

    public class PdfConversionException : DocumentConverterException
    {
        public PdfConversionException(string message) : base(message)
        {
        }

        public PdfConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}