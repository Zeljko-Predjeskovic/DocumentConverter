using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentConverter.Plugin.Shared.Picker;

namespace DocumentConverter.Plugin.Shared
{
    public interface IDocumentConverterService
    {
        Task<PdfFileInfo> ConvertPdfToSvgAsync(string filePath, string outputDir, int page = 1, CancellationToken cancellationToken = default);
        Task<string> ConvertPdfToSvgStringAsync(string filePath, int page = 1, CancellationToken cancellationToken = default);
        Task<string> PickDocumentAsync(DocumentPickerOptions pickerOptions);

    }
}