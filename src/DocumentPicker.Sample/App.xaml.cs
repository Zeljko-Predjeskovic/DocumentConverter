using DocumentConverter.Plugin.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DocumentPicker.Samples
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

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