#if MONOANDROID
using DocumentConverter.Plugin.Platforms.Android;
#endif
#if WINDOWS_UWP
using DocumentConverter.Plugin.Platforms.UniversalWindows;
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
#if MONOANDROID
            DocumentConverterProvider.FilePicker = new FilePickerAndroid(filePicker);
#else
            DocumentConverterProvider.FilePicker = filePicker;

#endif
#if WINDOWS_UWP
            DocumentConverterProvider.StreamProvider = new UniversalWindowsStreamProvider();
#else
            DocumentConverterProvider.StreamProvider = new StreamProviderImplementation();
#endif
        }
    }
}