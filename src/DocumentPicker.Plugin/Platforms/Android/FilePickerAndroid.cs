using System.Threading;
using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using DocumentConverter.Plugin.Shared.Picker;
using Xamarin.Essentials;
using FilePickerImplementation = DocumentConverter.Plugin.Shared.Picker.FilePickerImplementation;
using System.Linq;
using System.Net.Mime;

namespace DocumentConverter.Plugin.Platforms.Android
{
    public class FilePickerAndroid : IFilePicker
    {
        private readonly FilePickerImplementation _implementation;
        private readonly Context _context;
        private int _requestId;
        private TaskCompletionSource<string> _completionSource;
        private readonly string _tempFolderPath;



        private EventHandler<FilePickerEventArgs> Handler { get; }
        private EventHandler<EventArgs> CancelledHandler { get; }


        public FilePickerAndroid(FilePickerImplementation implementation)
        {
            _context = Application.Context;
            _implementation = implementation;
            _tempFolderPath = FileSystem.CacheDirectory + "DocumentConverter";


            Handler = (s, e) =>
            {
                var tcs = Interlocked.Exchange(ref _completionSource, null);

                using var stream = new MemoryStream();
                var path = StoreFileTemporaryAsync(e.Stream, e.FileName);
                FilePickerActivity.FilePicked -= Handler;

                tcs?.SetResult(path);
            };

            CancelledHandler = (s, e) => {
                var tcs = Interlocked.Exchange(ref _completionSource, null);

                FilePickerActivity.FilePickCancelled -= CancelledHandler;

                tcs?.SetResult(null);
            };

        }

        public async Task<string> PickAsync(DocumentPickerOptions options = null)
        {
            var version = (int)Build.VERSION.SdkInt;

            // Use Xamarin Essentials picker if version is under Android 13
            if (version < 33)
            {
                var storageWrite = new Permissions.StorageWrite();
                var writePermission = await storageWrite.RequestAsync();

                if ((int)writePermission != (int)PermissionStatus.Granted)
                {
                    throw new FilePickerPermissionException("No permission for android files!");
                }

                return await _implementation.PickAsync(options);
            }
            
            return await this.PickAsync(options);
        }

        private async Task<string> PickAsync()
        {
            try
            {
                var filePath = await TakeDocumentAsync();

                if (filePath == null)
                {
                    return null;
                }

                return filePath;
            }
            catch (Exception e)
            {
                throw new FilePickerException(e.Message, e);
            }
        }

        private Task<string> TakeDocumentAsync()
        {
            var id = GetRequestId();
            var ntcs = new TaskCompletionSource<string>(id);

            if (Interlocked.CompareExchange(ref _completionSource, ntcs, null) != null)
                throw new InvalidOperationException("Only one operation can be active at a time");

            try
            {
                var pickerIntent = new Intent(_context, typeof(FilePickerActivity));
                pickerIntent.SetFlags(ActivityFlags.NewTask);

                _context.StartActivity(pickerIntent);

                FilePickerActivity.FilePickCancelled += CancelledHandler;
                FilePickerActivity.FilePicked += Handler;
            }
            catch (Exception exAct)
            {
                return null;
            }

            return _completionSource.Task;
        }

        private int GetRequestId()
        {
            int id = _requestId;

            if (_requestId == int.MaxValue)
                _requestId = 0;
            else
                _requestId++;

            return id;
        }

        private string StoreFileTemporaryAsync(Stream sharedFileStream, string fileName)
        {
            var destinationFile = Path.Combine(_tempFolderPath, fileName);

            if (!Directory.Exists(_tempFolderPath)) Directory.CreateDirectory(_tempFolderPath);

            var bytes = new byte[sharedFileStream.Length];

            File.WriteAllBytes(destinationFile, bytes);


            return destinationFile;
        }
    }
}