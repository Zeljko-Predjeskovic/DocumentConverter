using DocumentConverter.Exceptions;
using PdfToSvg;
using Svg;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentConverter
{
    public interface IPdfToSvgConverter
    {
        Task ConvertPdfToSvgAsync(Stream openStream, Stream outputStream, CancellationToken cancellationToken = default);
    }
    
    public class PdfToSvgConverter : IPdfToSvgConverter
    {
        public async Task ConvertPdfToSvgAsync(Stream openStream, Stream outputStream, CancellationToken cancellationToken = default)
        {
            if (openStream == null)
                throw new ArgumentNullException("openStream", "cannot be null!");

            if (outputStream == null)
                throw new ArgumentNullException("outputDir", "cannot be null!");

            try
            {
                var doc = await PdfDocument.OpenAsync(openStream,
                    cancellationToken: cancellationToken);
                
                if(doc.Pages.Count != 1)
                    throw new MultiPagePdfException("Pdf document contains multiple pages - provide a single-page document");

                using var ms = new MemoryStream();
                await doc.Pages[0]
                    .SaveAsSvgAsync(ms,new SvgConversionOptions{FontResolver = FontResolver.EmbedOpenType},
                        cancellationToken: cancellationToken);
                ms.Seek(0, SeekOrigin.Begin);

                var svg = SvgDocument.Open<SvgDocument>(ms);
                if(SvgHasOnlyImages(svg)) 
                    throw new ScannedPdfException("The PDF page seems to be from a scanned document. Please upload this plan as image instead (.jpeg, .png)");

                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(outputStream);

            }
            catch (FileNotFoundException e)
            {
                throw new PdfNotFoundException(e.Message, e);
            }
            catch (PdfException e)
            {
                throw new PdfConversionException(e.Message, e);
            }
        }
        
        private bool SvgHasOnlyImages(SvgDocument svgDocument)
        {
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
