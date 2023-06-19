using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Svg;
using Microsoft.Maui.Controls.Xaml;
using FileSystem = Microsoft.Maui.Storage.FileSystem;
using System.Threading.Tasks;
using DocumentConverter.Exceptions;
using DocumentConverter.Plugin.Shared;
using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;
using Color = Svg.Interfaces.Color;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using IFilePicker = DocumentConverter.Plugin.Shared.Picker.IFilePicker;
using Image = Microsoft.Maui.Controls.Image;

namespace DocumentPicker.Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentPickerPage : ContentPage
    {
        private readonly IDocumentConverterService _converterService;
        private readonly IFilePicker _filePicker;
        private readonly ICustomStreamProvider _streamProvider;


        ObservableCollection<string> filePaths = new ObservableCollection<string>();
        
        public DocumentPickerPage()
        {
            _streamProvider = DocumentConverterProvider.StreamProvider;
            _converterService = new DocumentConverterService(_streamProvider);
            _filePicker = DocumentConverterProvider.FilePicker;

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
                        { DevicePlatform.WinUI, new[] { ".pdf" } }
                    }
                };

                try
                {
                    var filePath = await _filePicker.PickAsync(pickerOptions);
                    if (filePath != null)
                    {
                        filePaths.Add(filePath);
                    }
                }
                catch (FilePickerPermissionException)
                {
                    if (await DisplayAlert("Picker wont work!", "Please enable in settings!!", "ok", "cancel"))
                    {
                        Microsoft.Maui.ApplicationModel.AppInfo.ShowSettingsUI();
                    }
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
                Guid.NewGuid() + ".png");

            var taskResult = await Task.Run(async () =>
            {
                await progressBar.ProgressTo(0.3, 100, Easing.Linear);

                try
                {
                    using (var inputStream = await _streamProvider.OpenReadWriteAsync(file))
                    {
                        using (var outputStream = new MemoryStream())
                        {
                            await _converterService.ConvertPdfToSvgAsync(inputStream, outputStream);
                            outputStream.Seek(0, SeekOrigin.Begin);

                            var guide = Guid.NewGuid().ToString();
                            using (var x = File.Create(Path.Combine(Path.Combine(FileSystem.CacheDirectory, guide+ ".svg"))))
                            {

                            };

                            using (var stream = await _streamProvider.OpenReadWriteAsync(Path.Combine(FileSystem.CacheDirectory,
                                       guide + ".svg")))
                            {
                                await outputStream.CopyToAsync(stream);
                            }
                            outputStream.Seek(0, SeekOrigin.Begin);

                            var svgDoc = SvgDocument.Open<SvgDocument>(outputStream);


                            await progressBar.ProgressTo(0.75, 200, Easing.Linear);
                            using (var f = File.Create(newPath))
                            {
                                var bitMap = svgDoc.DrawDocument(backgroundColor: Color.Create(255, 255, 255),
                                    maxWidthHeight: 2000);
                                bitMap.SavePng(f, 100);
                                await progressBar.ProgressTo(1, 50, Easing.Linear);
                            }
                        }
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