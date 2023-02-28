using DocumentConverter.Exceptions;
using DocumentConverter.Plugin.Shared;
using DocumentConverter.Plugin.Shared.StreamProvider;
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
            CustomStreamProviderInitializer.Init();

            _documentConverterService = new DocumentConverterService(CustomStreamProvider.Instance);
        }

        [Test]
        public async Task WhenGivingPdfSource_ConvertToSvg()
        {
            //Arrange
            var filePath = ".\\Resources\\Vienna.pdf";
            File.Copy(filePath, ".\\Vienna_1.pdf", true);

            var outputDir = ".\\Resources";
            var expectedOutput = ".\\Resources\\Vienna_1.svg";


            //Act
            var result = await _documentConverterService.ConvertPdfToSvgAsync(".\\Vienna_1.pdf", outputDir);

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
            var exception = Assert.ThrowsAsync<PdfNotFoundException>(async () => await Action());
            Assert.That(exception.Message, Is.EqualTo("Could not find file '" + Path.GetFullPath(filePath) + "'.") );
            var exception2 = Assert.ThrowsAsync<PdfNotFoundException>(async () => await Action2());
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
            var exception = Assert.ThrowsAsync<PdfConversionException>(async () => await Action());
            Assert.That(exception.Message, Is.EqualTo("The specified file is not a valid PDF file. No file header was found."));
            var exception2 = Assert.ThrowsAsync<PdfConversionException>(async () => await Action2());
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
            var x = Assert.ThrowsAsync<ScannedPdfException>(async () => await docResult);
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
            var x = Assert.ThrowsAsync<MultiPagePdfException>(async () => await docResult);
            x.Message.ShouldBe("Pdf document contains multiple pages - provide a single-page document");
        }

        [Test]
        public async Task WhenGivingPdfStreamInput_ShouldReturnSvgStreamOutput()
        {
            //Arrange
            var svgStringExpected = await File.ReadAllTextAsync(".\\Resources\\Vienna.svg");

            await using var inputStream = File.Open(".\\Resources\\Vienna.pdf", FileMode.Open, FileAccess.Read);
            using var outputStream = new MemoryStream();
       
            
            //Act
            await _documentConverterService.ConvertPdfToSvgAsync(inputStream, outputStream);
            outputStream.Seek(0, SeekOrigin.Begin);
            //Assert
            var svgDocResult = SvgDocument.Open<SvgDocument>(outputStream);
            var svgDocExpected = SvgDocument.FromSvg<SvgDocument>(svgStringExpected);
            svgDocResult.Children.Count.ShouldBe(svgDocExpected.Children.Count);
            svgDocResult.Bounds.ShouldBe(svgDocExpected.Bounds);
            svgDocResult.Height.ShouldBe(svgDocExpected.Height);
        }

        [Test]
        public async Task WhenGivingInvalidPdfStreamInput_ShouldThrowException()
        {
            //Arrange
            await using var inputStream = File.Open(".\\Resources\\TextFile.txt", FileMode.Open, FileAccess.Read);
            using var outputStream = new MemoryStream();

            //Act
            var action = _documentConverterService.ConvertPdfToSvgAsync(inputStream, outputStream);

            //Assert
            var x = Assert.ThrowsAsync<PdfConversionException>(async () => await action);
            x.Message.ShouldBe("The specified file is not a valid PDF file. No file header was found.");
        }

        [Test]
        public async Task WhenGivingPdfWithManyPagesStreamInput_ShouldThrowException()
        {
            //Arrange
            await using var inputStream = File.Open(".\\Resources\\l1.pdf", FileMode.Open, FileAccess.Read);
            using var outputStream = new MemoryStream();

            //Act
            var action = _documentConverterService.ConvertPdfToSvgAsync(inputStream, outputStream);

            //Assert
            var x = Assert.ThrowsAsync<MultiPagePdfException>(async () => await action);
            x.Message.ShouldBe("Pdf document contains multiple pages - provide a single-page document");
        }


        [Test]
        public async Task WhenGivingScannedPdfStreamInput_ShouldThrowException()
        {
            //Arrange
            await using var inputStream = File.Open(".\\Resources\\Scanned.pdf", FileMode.Open, FileAccess.Read);
            using var outputStream = new MemoryStream();

            //Act
            var action = _documentConverterService.ConvertPdfToSvgAsync(inputStream, outputStream);

            //Assert
            var x = Assert.ThrowsAsync<ScannedPdfException>(async () => await action);
            x.Message.ShouldBe("The PDF page seems to be from a scanned document. Please upload this plan as image instead (.jpeg, .png)");
        }

        

        [Test]
        public async Task WhenGivingPdfSource_WithOpenFont_ConvertToSvg()
        {
            //Arrange
            var filePath = ".\\Resources\\Top3.pdf";
            File.Copy(filePath, ".\\Top3_1.pdf", true);

            var outputDir = ".\\Resources";
            var expectedOutput = ".\\Resources\\Top3_1.svg";


            //Act
            var result = await _documentConverterService.ConvertPdfToSvgAsync(".\\Top3_1.pdf", outputDir);

            //Assert
            Assert.That(result.PageCount, Is.EqualTo(1));
            Assert.That(result.ResultPath, Is.EqualTo(expectedOutput));
            Assert.That(File.Exists(expectedOutput));
            var input = File.ReadAllText(expectedOutput);
            Assert.That(result.Content, Is.EqualTo(input));
        }
    }
}