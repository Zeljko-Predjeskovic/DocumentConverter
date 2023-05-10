using System;

namespace DocumentConverter.Plugin.Shared.Picker
{
    public class FilePickerPermissionException : Exception
    {
        public FilePickerPermissionException (string message) : base(message) { }
        
    }
}