using System;
using System.IO;
using System.Threading.Tasks;
using DocumentConverter.Plugin.Shared.StreamProvider;
using Windows.Storage;

namespace DocumentPicker.UWP
{
    public class UwpStreamProviderImplementation : ICustomStreamProvider
    {
        public async Task<Stream> OpenReadAsync(string filePath)
        {
            return await OpenInternalAsync(filePath, FileAccessMode.Read);
        }

        public async Task<Stream> OpenReadWriteAsync(string filePath)
        {
            return await OpenInternalAsync(filePath, FileAccessMode.ReadWrite);
        }

        internal async Task<Stream> OpenInternalAsync(string filePath, FileAccessMode fileAccessMode)
        {
            var storageFile = await StorageFile.GetFileFromPathAsync(filePath);

            var raStream = await storageFile.OpenAsync(fileAccessMode);

            return raStream.AsStream();
        }
    }
}