using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DocumentConverter.Plugin.Shared.DocumentPicker
{
    public interface IDocumentPickerService
    {
        Task<FileResult> PickDocumentAsync(PickOptions pickOptions = null, CancellationToken token = default);
    }
}