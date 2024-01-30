using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared.Picker
{
    public interface IFilePicker
    {
        Task<FilePickerResult> PickAsync(DocumentPickerOptions options = null);
    }
}