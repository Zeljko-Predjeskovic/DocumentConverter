using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;
using PdfToSvg;
using Svg;

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

        public async Task<DocumentConverterResult> ConvertPdfToSvgAsync(string filePath, string outputDir, CancellationToken cancellationToken = default)
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
                    await doc.Pages[0]
                        .SaveAsSvgAsync(svgFile,
                            cancellationToken: cancellationToken);

                    var resultDocument = new DocumentConverterResult()
                    {
                        ResultPath = svgFile,
                        PageCount = doc.Pages.Count,
                        Content = File.ReadAllText(svgFile)

                    };

                    Validate(resultDocument);

                    return resultDocument;
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

        public async Task<DocumentConverterResult> ConvertPdfToSvgStringAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath", "cannot be null!");

            try
            {
                using (var doc = await PdfDocument.OpenAsync(await _customStreamProvider.OpenReadAsync(filePath),
                           cancellationToken: cancellationToken))
                {
                    var resultDocument = new DocumentConverterResult()
                    {
                        Content = await doc.Pages[0]
                            .ToSvgStringAsync(cancellationToken: cancellationToken),
                        PageCount = doc.Pages.Count
                    };

                    Validate(resultDocument);

                    return resultDocument;
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

        public void Validate(DocumentConverterResult result)
        {
            if (HasOverOnePage(result))
                throw new DocumentConverterException("Pdf document contains multiple pages - provide a single-page document");

            if (SvgHasOnlyImages(result.Content))
                throw new DocumentConverterException("The PDF page seems to be from a scanned document. Please upload this plan as image instead (.jpeg, .png)");
        }

        private bool HasOverOnePage(DocumentConverterResult result)
        {
            return result.PageCount > 1;
        }

        private bool SvgHasOnlyImages(string svgString)
        {
            var svgDocument = SvgDocument.FromSvg<SvgDocument>(svgString);

            var svgElements = svgDocument.GetDescendants().ToArray();
            var numberOfVisibleElementExceptImages = svgElements.OfType<SvgVisualElement>().Count(x => !(x is SvgImage) && !(x is SvgUse) && !(x is SvgGroup));
            var allImages = svgElements.OfType<SvgImage>().ToArray();

            // scanned documents normally only have one image on their page
            // and only very little elements
            var singleImageWithBase64Image = allImages.Length == 1 && allImages[0].IsBase64Image;

            return singleImageWithBase64Image && numberOfVisibleElementExceptImages == 0;
        }
    }
}