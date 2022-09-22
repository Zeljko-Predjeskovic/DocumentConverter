using System.Collections.Generic;
using Xamarin.Essentials;

namespace DocumentConverter.Plugin.Shared.Picker
{
    public class DocumentPickerOptions
    {
        public string PickerTitle { get; set; }

        public Dictionary<DevicePlatform, IEnumerable<string>> FileTypes { get; set; }
    }
}