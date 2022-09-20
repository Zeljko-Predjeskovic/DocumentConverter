using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DocumentConverter.Plugin.Shared.DocumentPicker
{
    public class DocumentPickerImplementation : IDocumentPickerService
    {
        public async Task<FileResult> PickDocumentAsync(PickOptions pickOptions = null, CancellationToken token = default)
        {
            try
            {
                var result = await FilePicker.PickAsync(pickOptions);
                if (result != null)
                {
                    //do something
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("" + ex.Message);
            }
        }
    }
}