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
        private readonly IFilePicker _filePicker;
        private readonly ICustomStreamProvider _streamProvider;


        ObservableCollection<string> filePaths = new ObservableCollection<string>();
        
        public DocumentPickerPage(ICustomStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
            _converterService = new DocumentConverterService(streamProvider);
            _filePicker = new FilePickerImplementation();

            InitializeComponent();

            filePaths.CollectionChanged += Files_CollectionChanged;

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

                var filePath = await _filePicker.PickAsync(pickerOptions);

                if(filePath != null)
                {
                    filePaths.Add(filePath);
                }
            };
        }

        private async void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (filePaths.Count == 0)
            {
                ImageList.Children.Clear();
                return;
            }
            if (e.NewItems.Count == 0)
                return;

            var file = e.NewItems[0] as string;

            List<string> pageStrings = new List<string>();

            int page = 1;

            var progressBar = new ProgressBar();
            Grid.Children.Add(progressBar);

            var newPath = Path.Combine(FileSystem.CacheDirectory,
                Path.GetFileNameWithoutExtension(file) + ".png");

            var taskResult = await Task.Run(async () =>
            {
                await progressBar.ProgressTo(0.3, 100, Easing.Linear);

                try
                {
                    var inputStream = await _streamProvider.OpenReadWriteAsync(file);
                    var outputStream = new MemoryStream();

                    await _converterService.ConvertPdfToSvgAsync(inputStream, outputStream);
                    outputStream.Seek(0, SeekOrigin.Begin);

                    var svgDoc = SvgDocument.Open<SvgDocument>(outputStream);
                    

                    await progressBar.ProgressTo(0.75, 200, Easing.Linear);
                    using (var f = File.Create(newPath))
                    {
                        var bitMap = svgDoc.DrawAllContents();
                        bitMap.SavePng(f, 100);
                        await progressBar.ProgressTo(1, 50, Easing.Linear);
                    }

                    return (newPath, true);
                }
                catch (DocumentConverterException ex)
                {
                    return (ex.Message, false);
                }

            });
            if (!taskResult.Item2)
            {
                await DisplayAlert("Converting failed! ", taskResult.Item1, "OK", "Cancel");
                Grid.Children.Remove(progressBar);

                return;
            }
            var image = new Image { WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.AspectFit };
            image.Source = ImageSource.FromFile(newPath);

            ImageList.Children.Add(image);

            Grid.Children.Remove(progressBar);
        }
    }
}