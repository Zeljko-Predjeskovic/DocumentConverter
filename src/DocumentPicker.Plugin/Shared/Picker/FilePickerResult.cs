using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace DocumentConverter.Plugin.Shared.Picker
{
    public class FilePickerResult : IDisposable
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }

        public FilePickerResult(string fileName, string fullPath)
        {
            FileName = fileName;
            FullPath = fullPath;
        }
        
        internal Action OnDispose { get; set; } = (() => { return; });

        public void Dispose()
        {
            OnDispose();
        }
    }
}