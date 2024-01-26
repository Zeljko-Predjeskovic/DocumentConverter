using System;
using System.IO;

namespace DocumentConverter.Plugin.Platforms.Android
{
    public class FilePickerEventArgs : EventArgs
    {
        public string FileName { get; set; }

        public Stream Stream { get; set; }

        public FilePickerEventArgs()
        {

        }
        public FilePickerEventArgs(string fileName,Stream stream)
        {
            Stream = stream;
            FileName = fileName;
        }
    }
}