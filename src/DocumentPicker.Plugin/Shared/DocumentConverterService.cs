using DocumentConverter.Plugin.Shared.StreamProvider;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocumentConverter.Exceptions;

namespace DocumentConverter.Plugin.Shared
{
    public class DocumentConverterService : PdfToSvgConverter, IDocumentConverterService
    {
        private readonly ICustomStreamProvider _customStreamProvider;

        public DocumentConverterService(ICustomStreamProvider customStreamProvider)
        {
            _customStreamProvider = customStreamProvider;
        }

        public async Task<DocumentConverterResult> ConvertPdfToSvgAsync(string filePath, string outputDir, CancellationToken cancellationToken = default)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath", "cannot be null!");

            if (outputDir == null) 
                throw new ArgumentNullException("outputDir", "cannot be null!");

            try
            {
                var svgFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(filePath) + ".svg");
                using (var openStream = await _customStreamProvider.OpenReadAsync(filePath))
                using (var outputStream = File.OpenWrite(svgFile))
                {
                    await base.ConvertPdfToSvgAsync(openStream, outputStream, cancellationToken);
                }
                return new DocumentConverterResult()
                {
                    ResultPath = svgFile,
                    PageCount = 1,
                    Content = File.ReadAllText(svgFile)

                };
            }
            catch (FileNotFoundException x)
            {
                throw new PdfNotFoundException(x.Message, x);
            }
        }

        public async Task<DocumentConverterResult> ConvertPdfToSvgStringAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath", "cannot be null!");

            try
            {
                using (var openStream = await _customStreamProvider.OpenReadAsync(filePath))
                using (var outputStream = new MemoryStream())
                {
                    await base.ConvertPdfToSvgAsync(openStream, outputStream, cancellationToken);

                    outputStream.Seek(0, SeekOrigin.Begin);

                    var resultDocument = new DocumentConverterResult()
                    {
                        Content = Encoding.UTF8.GetString(outputStream.ToArray()),
                        PageCount = 1
                    };
                    return resultDocument;
                }
            }
            catch (FileNotFoundException x)
            {
                throw new PdfNotFoundException(x.Message, x);
            }
        }
    }
}