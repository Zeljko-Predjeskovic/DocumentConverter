using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DocumentPicker.Plugin.Shared;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DocumentPicker.Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentPickerPage : ContentPage
    {
        ObservableCollection<FileResult> files = new ObservableCollection<FileResult>();
        public DocumentPickerPage()
        {
            InitializeComponent();

            //files.CollectionChanged += Files_CollectionChanged;

            pickDocument.Clicked += async (sender, args) =>
            {
                var x = new DocumentPickerService();

                var fileResult = await x.PickDocumentAsync();
                
                files.Add(fileResult);
            };
        }

   //     private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
   //     {
   //         if (files.Count == 0)
   //         {
   //             ImageList.Children.Clear();
   //             return;
   //         }
   //         if (e.NewItems.Count == 0)
   //             return;

   //         var file = e.NewItems[0] as DocumentFile;
   //         var image = new Image { WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.AspectFit };
   //         if (file != null)
   //         {
   //             image.Source = ImageSource.FromFile(file.FileResult.FullPath);
   //             /*image.Source = ImageSource.FromStream(() =>
   //{
   //    var stream = file.GetStream();
   //    return stream;
   //});*/
   //             ImageList.Children.Add(image);

   //             var image2 = new CachedImage { WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.AspectFit };
   //             image2.Source = ImageSource.FromFile(file.FileResult.FullPath);
   //             ImageList.Children.Add(image2);
   //         }
   //     }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContentPage());
        }
    }
}