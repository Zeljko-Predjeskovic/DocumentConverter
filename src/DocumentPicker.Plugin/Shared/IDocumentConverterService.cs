using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared
{
    public interface IDocumentConverterService
    {
        Task<DocumentConverterResult> ConvertPdfToSvgAsync(string filePath, string outputDir, CancellationToken cancellationToken = default);
        Task<DocumentConverterResult> ConvertPdfToSvgStringAsync(string filePath, CancellationToken cancellationToken = default);
        Task ConvertPdfToSvgAsync(Stream inputStream, Stream outputStream ,CancellationToken cancellationToken = default);

    }
}