using System;
using System.IO;
using System.Threading.Tasks;
using DocumentConverter.Plugin.Shared.StreamProvider;
using Windows.Storage;

namespace DocumentPicker.UWP
{
    public class UwpStreamProviderImplementation : ICustomStreamProvider
    {
        public async Task<Stream> OpenAsync(string filePath)
        {
            var storageFile = await StorageFile.GetFileFromPathAsync(filePath);

            var raStream = await storageFile.OpenReadAsync();
            
            return raStream.AsStream();

        }
    }
}