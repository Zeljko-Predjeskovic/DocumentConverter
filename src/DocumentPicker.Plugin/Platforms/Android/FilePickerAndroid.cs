using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using DocumentConverter.Plugin.Shared.Picker;

namespace DocumentConverter.Plugin.Platforms.Android
{
    public class FilePickerAndroid : IFilePicker
    {
        private readonly FilePickerImplementation _implementation;

        public FilePickerAndroid(FilePickerImplementation implementation)
        {
            _implementation = implementation;
        }

        public async Task<string> PickAsync(DocumentPickerOptions options = null)
        {

            if (Application.Context.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                throw new FilePickerPermissionException("No permission for android files!");
            }

            return await _implementation.PickAsync(options);
        }
    }
}