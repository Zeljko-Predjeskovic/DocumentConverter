using System;
using Xamarin.Essentials;

namespace DocumentPicker.Plugin.Shared
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