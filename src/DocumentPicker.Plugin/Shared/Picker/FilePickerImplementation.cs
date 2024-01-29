using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DocumentConverter.Plugin.Shared.Picker
{
    public class FilePickerImplementation : IFilePicker
    {
        public async Task<FilePickerResult> PickAsync(DocumentPickerOptions pickerOptions = null)
        {
            if (pickerOptions == null)
            {
                pickerOptions = new DocumentPickerOptions()
                {
                    PickerTitle = "Please pick a pdf file",
                    FileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "com.adobe.pdf" } }, // or general UTType values
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.UWP, new[] { ".pdf" } }
                    }
                };
            }

            try
            {
                var options = new PickOptions()
                {
                    PickerTitle = pickerOptions.PickerTitle,
                    FileTypes = new FilePickerFileType(pickerOptions.FileTypes)
                };

                var fileResult = await FilePicker.PickAsync(options);

                if (fileResult == null) 
                    return null;
                
                return new FilePickerResult(fileResult.FileName,fileResult.FullPath);
            }

            catch (Exception e)
            {
                throw new FilePickerException(e.Message, e);
            }
        }
    }
}