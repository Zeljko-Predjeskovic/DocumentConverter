using System;

namespace DocumentConverter.Plugin.Shared.Picker
{
    public class FilePickerException : Exception
    {
        public FilePickerException(string message) : base(message) { }
        public FilePickerException(string message, Exception innerException) : base(message, innerException) { }
    }
}