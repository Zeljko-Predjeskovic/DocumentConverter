using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;
using PdfToSvg;

namespace DocumentConverter.Plugin.Shared
{
    public class DocumentConverterService : IDocumentConverterService
    {

        private readonly IFilePicker _filePicker;
        private readonly ICustomStreamProvider _customStreamProvider;

        public DocumentConverterService(IFilePicker filePicker, ICustomStreamProvider customStreamProvider)
        {
            _filePicker = filePicker;
            _customStreamProvider = customStreamProvider;
        }

        public async Task<PdfFileInfo> ConvertPdfToSvgAsync(string filePath, string outputDir, int page = 1, CancellationToken cancellationToken = default)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath", "cannot be null!");

            if (outputDir == null) 
                throw new ArgumentNullException("outputDir", "cannot be null!");

            try
            {
                using (var doc = await PdfDocument.OpenAsync(filePath, cancellationToken: cancellationToken))
                {
                    var svgFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(filePath) + ".svg");
                    await doc.Pages[page-1]
                        .SaveAsSvgAsync(svgFile,
                            cancellationToken: cancellationToken);

                    return new PdfFileInfo()
                    {
                        Title = doc.Info.Title,
                        Author = doc.Info.Author,
                        FilePath = svgFile,
                        PageCount = doc.Pages.Count
                    };
                }
            }
            catch (FileNotFoundException e)
            {
                throw new DocumentConverterException(e.Message, e);
            }
            catch (PdfException e)
            {
                throw new DocumentConverterException(e.Message, e);
            }
        }

        public async Task<string> ConvertPdfToSvgStringAsync(string filePath, int page = 1, CancellationToken cancellationToken = default)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath", "cannot be null!");

            try
            {
                using (var doc = await PdfDocument.OpenAsync(await _customStreamProvider.OpenReadAsync(filePath), cancellationToken: cancellationToken))
                {
                    return await doc.Pages[page - 1]
                        .ToSvgStringAsync(cancellationToken: cancellationToken);
                }
            }
            catch (FileNotFoundException e)
            {
                throw new DocumentConverterException(e.Message, e);
            }
            catch (PdfException e)
            {
                throw new DocumentConverterException(e.Message, e);
            }
        }

        public async Task<string> PickDocumentAsync(DocumentPickerOptions pickerOptions = null)
        {

            var filePath = await _filePicker.PickAsync(pickerOptions);

            return filePath;
        }
    }
}