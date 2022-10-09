using DocumentConverter.Plugin.Shared.StreamProvider;
using DocumentPicker.Samples.NotifyVisibility;
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

            CustomStreamProviderInitializer.Init();
            MainPage = new NavigationPage(new DocumentPickerPage());
            SharedVisibles.Init();
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