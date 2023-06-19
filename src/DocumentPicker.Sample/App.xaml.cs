using DocumentConverter.Plugin.Shared;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Svg;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DocumentPicker.Samples
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            SvgPlatform.Init();
            DocumentConverterInitializer.Init();

            MainPage = new NavigationPage(new DocumentPickerPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}