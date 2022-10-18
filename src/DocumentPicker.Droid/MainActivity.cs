using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DocumentConverter.Plugin.Shared;
using DocumentConverter.Plugin.Shared.FileSharing;
using DocumentConverter.Plugin.Shared.StreamProvider;
using DocumentPicker.Samples;
using DocumentPicker.Samples.NotifyVisibility;
using Svg;

namespace DocumentPicker.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"application/pdf")]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            LoadApp(bundle);


            if (Intent.ActionSend.Equals(Intent?.Action) &&
                Intent.Type != null &&
                "application/pdf".Equals(Intent.Type))
            {
                // Get the info from ClipData 
                var pdf = Intent.ClipData.GetItemAt(0);

                // Open a stream from the URI 
                var pdfStream = ContentResolver.OpenInputStream(pdf.Uri);

                SharedStream.Instance = pdfStream;
                SharedVisibles.ShowShareViews();
            }
        }

        private void LoadApp(Bundle bundle)
        {
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Xamarin.Essentials.Platform.Init(this, bundle);
            Xamarin.Forms.Forms.Init(this, bundle);
            SvgPlatform.Init();
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}