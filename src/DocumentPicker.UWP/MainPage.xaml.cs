using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Storage;
using DocumentConverter.Plugin.Shared.StreamProvider;

namespace DocumentPicker.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Samples.App());
        }

    }
}
