using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared.Picker
{
    public interface IFilePicker
    {
        Task<string> PickAsync(DocumentPickerOptions options = null);
    }
}