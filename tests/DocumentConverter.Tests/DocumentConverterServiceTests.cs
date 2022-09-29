using DocumentConverter.Plugin.Shared;
using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;
using Moq;
using Shouldly;
using Svg;

namespace DocumentConverter.Tests
{
    public class DocumentConverterServiceTests
    {

        private IDocumentConverterService _documentConverterService;

        [SetUp]
        public void Setup()
        {
            SvgPlatform.Init();
            var streamProvider = new StreamProviderImplementation();
            CrossCallingStreamProvider.RegisterStreamProvider(streamProvider);

            _documentConverterService = new DocumentConverterService(streamProvider);
        }

        [Test]
        public async Task WhenGivingPdfSource_ConvertToSvg()
        {
            //Arrange
            var filePath = ".\\Resources\\Vienna.pdf";
            var outputDir = ".\\Resources";
            var expectedOutput = ".\\Resources\\Vienna.svg";

            //Act
            var result = await _documentConverterService.ConvertPdfToSvgAsync(filePath, outputDir);

            //Assert
            Assert.That(result.PageCount, Is.EqualTo(1));
            Assert.That(result.ResultPath, Is.EqualTo(expectedOutput));
            Assert.That(File.Exists(expectedOutput));
            var input = File.ReadAllText(expectedOutput);
            Assert.That(result.Content, Is.EqualTo(input));
        }

        [Test]
        public async Task WhenGivingPdfSource_ConvertToSvgString()
        {
            //Arrange
            var filePath = ".\\Resources\\Vienna.pdf";

            //Act
            var svgString = await _documentConverterService.ConvertPdfToSvgStringAsync(filePath);

            //Assert
            Assert.That(svgString.Content.Contains("<svg"));
            Assert.That(svgString.Content.Contains("</svg>"));
        }

        [Test]
        public void WhenGivingNotExistingSource_ThenThrowsException()
        {
            //Arrange
            var filePath = ".\\Resources\\abc.pdf";
            var outputDir = ".\\Resources";

            //Act
            async Task Action() => await _documentConverterService.ConvertPdfToSvgAsync(filePath, outputDir);
            async Task Action2() => await _documentConverterService.ConvertPdfToSvgStringAsync(filePath);


            //Assert
            var exception = Assert.ThrowsAsync<DocumentConverterException>(async () => await Action());
            Assert.That(exception.Message, Is.EqualTo("Could not find file '" + Path.GetFullPath(filePath) + "'.") );
            var exception2 = Assert.ThrowsAsync<DocumentConverterException>(async () => await Action2());
            Assert.That(exception2.Message, Is.EqualTo("Could not find file '" + Path.GetFullPath(filePath) + "'.") );
        }

        [Test]
        public void WhenFileNotPdf_FileShouldBeInvalid()
        {
            //Arrange
            var filePath = ".\\Resources\\TextFile.txt";
            var outputDir = ".\\Resources";

            //Act
            async Task Action() => await _documentConverterService.ConvertPdfToSvgAsync(filePath, outputDir);
            async Task Action2() => await _documentConverterService.ConvertPdfToSvgStringAsync(filePath);

            //Assert
            var exception = Assert.ThrowsAsync<DocumentConverterException>(async () => await Action());
            Assert.That(exception.Message, Is.EqualTo("The specified file is not a valid PDF file. No file header was found."));
            var exception2 = Assert.ThrowsAsync<DocumentConverterException>(async () => await Action2());
            Assert.That(exception2.Message, Is.EqualTo("The specified file is not a valid PDF file. No file header was found."));
        }


        [Test]
        public void WhenGivingScannedPdfFile_ShouldNotConvertToPdf()
        {
            //Arrange
            var filePath = ".\\Resources\\Scanned.pdf";
            var outputDir = ".\\Resources";

            //Act
            var docResult =  _documentConverterService.ConvertPdfToSvgAsync(filePath, outputDir);

            //Assert
            var x = Assert.ThrowsAsync<DocumentConverterException>(async () => await docResult);
            x.Message.ShouldBe("The PDF page seems to be from a scanned document. Please upload this plan as image instead (.jpeg, .png)");
        }

        [Test]
        public void WhenGivingPdfFileWithOverOnePage_ShouldNotConvertToPdf()
        {
            //Arrange
            var filePath = ".\\Resources\\l1.pdf";
            var outputDir = ".\\Resources";

            //Act
            var docResult = _documentConverterService.ConvertPdfToSvgAsync(filePath, outputDir);

            //Assert
            var x = Assert.ThrowsAsync<DocumentConverterException>(async () => await docResult);
            x.Message.ShouldBe("Pdf document contains multiple pages - provide a single-page document");
        }
    }
}