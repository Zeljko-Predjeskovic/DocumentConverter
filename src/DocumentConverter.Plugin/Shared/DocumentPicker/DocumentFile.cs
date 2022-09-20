using System;
using Xamarin.Essentials;

namespace DocumentConverter.Plugin.Shared.DocumentPicker
{
    public class DocumentFile
    {
        public DocumentFile(FileResult fileResult)
        {
            FileResult = fileResult;
        }

        public FileResult FileResult { get; private set; }
    }
}