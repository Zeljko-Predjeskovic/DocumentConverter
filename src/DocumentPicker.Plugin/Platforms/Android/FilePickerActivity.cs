using Android.App;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using System;
using Android.Content;
using Uri = Android.Net.Uri;

namespace DocumentConverter.Plugin.Platforms.Android
{
    [Activity]
    public class FilePickerActivity : Activity
    {
        private Context context;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            context = Application.Context;


            var intent = new Intent(Intent.ActionGetContent);
            intent.SetType("application/pdf");

            
            StartActivityForResult(Intent.CreateChooser(intent, "Select file"), 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Canceled || data == null)
            {
                OnFilePickCancelled();
                Finish();
            }
            else
            {
                try
                {
                    var uri = data.Data;

                    var filename = GetFileName(context, uri);

                    using var stream = ContentResolver?.OpenInputStream(uri);

                    OnFilePicked(new FilePickerEventArgs(filename,stream));
                }
                catch (Exception e)
                {
                    // Notify user file picking failed.
                    OnFilePickCancelled();
                }
                finally
                {
                    Finish();
                }
            }
        }

        string GetFileName(Context ctx, Uri uri)
        {

            string[] projection = { MediaStore.MediaColumns.DisplayName };

            var cr = ctx.ContentResolver;
            var name = "";
            var metaCursor = cr.Query(uri, projection, null, null, null);

            if (metaCursor != null)
            {
                try
                {
                    if (metaCursor.MoveToFirst())
                    {
                        name = metaCursor.GetString(0);
                    }
                }
                finally
                {
                    metaCursor.Close();
                }
            }
            return name;
        }

        internal static event EventHandler<FilePickerEventArgs> FilePicked;
        internal static event EventHandler<EventArgs> FilePickCancelled;

        private void OnFilePickCancelled()
        {
            FilePickCancelled?.Invoke(null, null);
        }

        private void OnFilePicked(FilePickerEventArgs e)
        {
            var picked = FilePicked;

            if (picked != null)
                picked(null, e);
        }
    }
}