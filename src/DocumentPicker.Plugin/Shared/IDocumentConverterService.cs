using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentConverter.Plugin.Shared.Picker;

namespace DocumentConverter.Plugin.Shared
{
    public interface IDocumentConverterService
    {
        Task<DocumentConverterResult> ConvertPdfToSvgAsync(string filePath, string outputDir, int page = 0, CancellationToken cancellationToken = default);
        Task<DocumentConverterResult> ConvertPdfToSvgStringAsync(string filePath, int page = 0, CancellationToken cancellationToken = default);

    }
}