using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentConverter.Plugin.Shared.StreamProvider;
using PdfToSvg;
using Svg;

namespace DocumentConverter.Plugin.Shared
{
    public static class DocumentConversionExtension
    {
        public static async Task<PdfFileInfo> GetPdfFileInfo(string filePath)
        {
            var x = Path.GetExtension(filePath);
            if (!Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new DocumentConverterException("File is not a Pdf!");
            }

            try
            {
                using (var doc = await PdfDocument.OpenAsync(await CrossCallingStreamProvider.OpenAsync(filePath)))
                {

                    return new PdfFileInfo()
                    {
                        Title = doc.Info.Title,
                        Author = doc.Info.Author,
                        FilePath = filePath,
                        PageCount = doc.Pages.Count
                    };
                }
            }
            catch (PdfException e)
            {
                throw new DocumentConverterException("Open File failed!", e);
            }
        }

        public static bool HasOverOnePage(this PdfFileInfo conversionResult)
        {
            return conversionResult.PageCount > 1;
        }

        public static bool SvgHasOnlyImages(SvgDocument svgDocument)
        {
            var elements = svgDocument.GetDescendants();
            var svgElements = elements as SvgElement[] ?? elements.ToArray();
            return svgElements.All(element => element.ElementName != "path");
        }
    }
}