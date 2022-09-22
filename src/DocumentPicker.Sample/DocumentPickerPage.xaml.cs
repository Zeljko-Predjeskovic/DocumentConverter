using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Svg;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FileSystem = Xamarin.Essentials.FileSystem;
using Image = Xamarin.Forms.Image;
using System.Threading.Tasks;
using DocumentConverter.Plugin.Shared;
using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;
using Xamarin.Essentials;

namespace DocumentPicker.Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentPickerPage : ContentPage
    {
        private readonly IDocumentConverterService _converterService;
        ObservableCollection<PdfFileInfo> files = new ObservableCollection<PdfFileInfo>();
        
        public DocumentPickerPage(ICustomStreamProvider streamProvider)
        {
            _converterService = new DocumentConverterService(new FilePickerImplementation(), streamProvider);

            InitializeComponent();

            files.CollectionChanged += Files_CollectionChanged;

            pickDocument.Clicked += async (sender, args) =>
            {
                var pickerOptions = new DocumentPickerOptions()
                {
                    PickerTitle = "Please pick a pdf file",
                    FileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "com.adobe.pdf" } }, // or general UTType values
                        { DevicePlatform.Android, new[] { "application/pdf", "application/png", "application/svg" } },
                        { DevicePlatform.UWP, new[] { ".pdf" } }
                    }
                };

                var fileResult = await _converterService.PickDocumentAsync(pickerOptions);

                if(fileResult != null)
                {
                    files.Add(await DocumentConversionExtension.GetPdfFileInfo(fileResult));
                }
            };
        }

        private async void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (files.Count == 0)
            {
                ImageList.Children.Clear();
                return;
            }
            if (e.NewItems.Count == 0)
                return;

            var file = e.NewItems[0] as PdfFileInfo;

            List<string> pageStrings = new List<string>();

            for (int i = 1; i <= file.PageCount; i++)
            {
                pageStrings.Add(i.ToString());
            }

            int page = 1;
            if (file.HasOverOnePage())
                page = Convert.ToInt32(await DisplayActionSheet("Please choose a Page", "Your document has more than one page.", null, pageStrings.ToArray())) ;

            var progressBar = new ProgressBar();
            Grid.Children.Add(progressBar);

            var newPngFilePath = await Task.Run(async () =>
            {
                await progressBar.ProgressTo(0.3, 100, Easing.Linear);
                var svgString = await _converterService.ConvertPdfToSvgStringAsync(file?.FilePath, page);

                var svgDoc = SvgDocument.FromSvg<SvgDocument>(svgString);

                if (DocumentConversionExtension.SvgHasOnlyImages(svgDoc))
                {
                    return null;
                }

                var newPath = Path.Combine(FileSystem.CacheDirectory, Path.GetFileNameWithoutExtension(file?.FilePath) + ".png");

                await progressBar.ProgressTo(0.75, 200, Easing.Linear);
                using (var f = File.Create(newPath))
                {
                    var bitMap = svgDoc.DrawAllContents();
                    bitMap.SavePng(f, 100);
                    await progressBar.ProgressTo(1, 50, Easing.Linear);
                }
                return newPath; 
            });

            if (newPngFilePath == null)
            {
                await DisplayAlert("Pdf is no valid!", "Please do not use pdfs with only one image! Instead, please load the image file", "OK", "Cancel");
                Grid.Children.Remove(progressBar);
                return;
            }
            var image = new Image { WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.AspectFit };
            image.Source = ImageSource.FromFile(newPngFilePath);

            ImageList.Children.Add(image);

            Grid.Children.Remove(progressBar);
        }
    }
}