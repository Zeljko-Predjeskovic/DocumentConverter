using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DocumentPicker.Plugin.Shared
{
    public interface IDocumentPickerService
    {
        Task<FileResult> PickDocumentAsync();
    }
}