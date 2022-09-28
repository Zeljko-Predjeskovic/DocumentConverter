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

        public async Task<DocumentConverterResult> ConvertPdfToSvgAsync(string filePath, string outputDir, int page = 0, CancellationToken cancellationToken = default)
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
                    await doc.Pages[page]
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

        public async Task<DocumentConverterResult> ConvertPdfToSvgStringAsync(string filePath, int page = 0, CancellationToken cancellationToken = default)
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
                        Content = await doc.Pages[page]
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
                throw new DocumentConverterException("Pdf Plan should not have more than one page!");

            if (SvgHasOnlyImages(result.Content))
                throw new DocumentConverterException("Pdf Plan should not be scanned!");
        }

        private bool HasOverOnePage(DocumentConverterResult result)
        {
            return result.PageCount > 1;
        }

        private bool SvgHasOnlyImages(string svgString)
        {
            var svgDocument = SvgDocument.FromSvg<SvgDocument>(svgString);

            var elements = svgDocument.GetDescendants();
            var svgElements = elements as SvgElement[] ?? elements.ToArray();

            return svgElements.Count(element => element is SvgImage) == 1 && svgElements.All(element => element.ElementName != "path");

            //if (svgDocument.Children.Count(element => element is SvgImage) == 1 && svgDocument.Children[0] is SvgImage svgImg)
            //{
            //    return svgImg.Href != null && svgImg.Href.ToString().StartsWith("data:image/jpeg;base64");
            //}

            //return false;
        }
    }
}