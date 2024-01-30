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
using DocumentConverter.Exceptions;
using System.Collections.Generic;

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

                if (Path.GetExtension(e.FileName) != ".pdf")
                {
                    throw new PdfConversionException("Invalid file! Please pick a pdf!");
                }

                using var stream = new MemoryStream();

                var path = StoreFileTemporaryAsync(e.Stream, e.FileName);
                FilePickerActivity.FilePicked -= Handler;

                tcs?.SetResult(e.FileName);
            };

            CancelledHandler = (s, e) =>
            {
                var tcs = Interlocked.Exchange(ref _completionSource, null);

                FilePickerActivity.FilePickCancelled -= CancelledHandler;

                tcs?.SetResult(null);
            };
        }

        public async Task<FilePickerResult> PickAsync(DocumentPickerOptions options = null)
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

            var fileName = await this.PickAsync();

            var filePickerResult = new FilePickerResult(fileName, Path.Combine(_tempFolderPath, fileName));
            filePickerResult.OnDispose = ClearTempFolder;

            return filePickerResult;
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

            using var destinationStream = File.OpenWrite(destinationFile);
            sharedFileStream.CopyTo(destinationStream);

            return destinationFile;
        }

        private void ClearTempFolder()
        {
            if (!Directory.Exists(_tempFolderPath))
                return;

            foreach (var f in Directory.EnumerateFileSystemEntries(_tempFolderPath, "*.*", SearchOption.AllDirectories)
                         .Where(File.Exists))
            {
                try
                {
                    File.Delete(f);
                }
                catch (Exception x)
                {
                    throw new FilePickerException($"Failed deleting folder {_tempFolderPath}!");
                }
            }
        }
    }
}