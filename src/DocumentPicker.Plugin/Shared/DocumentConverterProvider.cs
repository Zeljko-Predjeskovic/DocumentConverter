using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;
using IFilePicker = DocumentConverter.Plugin.Shared.Picker.IFilePicker;

namespace DocumentConverter.Plugin.Shared
{
    public static class DocumentConverterProvider
    {
        public static ICustomStreamProvider StreamProvider { get; set; }

        public static IFilePicker FilePicker { get; set; }

    }
}