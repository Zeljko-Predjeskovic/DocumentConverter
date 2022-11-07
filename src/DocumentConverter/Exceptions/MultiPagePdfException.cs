using System;

namespace DocumentConverter.Exceptions
{

    public class MultiPagePdfException : DocumentConverterException
    {
        public MultiPagePdfException(string message) : base(message)
        {
        }

        public MultiPagePdfException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}