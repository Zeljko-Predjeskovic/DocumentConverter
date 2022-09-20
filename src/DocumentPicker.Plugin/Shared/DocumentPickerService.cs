using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DocumentPicker.Plugin.Shared
{
    public class DocumentPickerService : IDocumentPickerService
    {
        public async Task<FileResult> PickDocumentAsync()
        {
            try
            {
                var customFileType =
                    new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    { { DevicePlatform.iOS, new[] { "com.adobe.pdf" } }, // or general UTType values
                        { DevicePlatform.Android, new[] { "application/pdf"} },
                        { DevicePlatform.UWP, new[] { ".pdf"} }
                    });

                var options = new PickOptions()
                {
                    PickerTitle = "Please select a pdf file",
                    FileTypes = customFileType
                };

                return await FilePicker.PickAsync(options);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("" + ex.Message);
            }
        }
    }
}