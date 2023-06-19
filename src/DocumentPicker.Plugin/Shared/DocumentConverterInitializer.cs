#if ANDROID
using DocumentConverter.Plugin.Platforms.Android;
#endif
#if WINDOWS
using DocumentConverter.Plugin.Platforms.Windows;
using Windows.Storage;
# endif
using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;

namespace DocumentConverter.Plugin.Shared
{
    public static class DocumentConverterInitializer
    {
        public static void Init()
        {
            var filePicker = new FilePickerImplementation();
#if ANDROID
            DocumentConverterProvider.FilePicker = new FilePickerAndroid(filePicker);
#else
            DocumentConverterProvider.FilePicker = filePicker;

#endif
#if WINDOWS
            DocumentConverterProvider.StreamProvider = new UniversalWindowsStreamProvider();
#else
            DocumentConverterProvider.StreamProvider = new StreamProviderImplementation();
#endif
        }
    }

}