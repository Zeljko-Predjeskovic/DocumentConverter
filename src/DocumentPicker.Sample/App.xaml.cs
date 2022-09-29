using DocumentConverter.Plugin.Shared.StreamProvider;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DocumentPicker.Samples
{
    public partial class App : Application
    {
        public App(ICustomStreamProvider streamProvider)
        {
            InitializeComponent();

            CrossCallingStreamProvider.RegisterStreamProvider(streamProvider);
            MainPage = new NavigationPage(new DocumentPickerPage(streamProvider));
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