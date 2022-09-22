using DocumentConverter.Plugin.Shared;
using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;
using Moq;

namespace DocumentConverter.Tests
{
    public class DocumentConverterServiceTests
    {

        private readonly Mock<IFilePicker> _filePickerMock = new ();
        private IDocumentConverterService _documentConverterService;

        [SetUp]
        public void Setup()
        {
            var streamProvider = new StreamProviderImplementation();
            CrossCallingStreamProvider.RegisterStreamProvider(streamProvider);

            _documentConverterService = new DocumentConverterService(_filePickerMock.Object, streamProvider);
        }

        [Test]
        public async Task WhenGivingPdfSource_ConvertToSvg()
        {
            //Arrange
            var filePath = ".\\Resources\\l1.pdf";
            var outputDir = ".\\Resources";
            var expectedOutput = ".\\Resources\\l1.svg";

            //Act
            await _documentConverterService.ConvertPdfToSvgAsync(filePath, outputDir);

            //Assert
            Assert.That(File.Exists(expectedOutput));
            var input = await File.ReadAllLinesAsync(expectedOutput);
            Assert.That(input.Any(s => s.Contains("<svg")));
            Assert.That(input.Any(s => s.Contains("</svg>")));
        }

        [Test]
        public async Task WhenGivingPdfSource_ConvertToSvgString()
        {
            //Arrange
            var filePath = ".\\Resources\\l1.pdf";

            //Act
            var svgString = await _documentConverterService.ConvertPdfToSvgStringAsync(filePath);

            //Assert
            Assert.That(svgString.Contains("<svg"));
            Assert.That(svgString.Contains("</svg>"));
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
        public async Task WhenGivingScannedPdfFile_ShouldConvertToPdf()
        {
            //Arrange
            var filePath = ".\\Resources\\Scanned.pdf";
            var outputDir = ".\\Resources";
            var expectedOutput = ".\\Resources\\Scanned.svg";

            //Act
            var docResult = await _documentConverterService.ConvertPdfToSvgAsync(filePath, outputDir);

            //Assert
            Assert.That(File.Exists(expectedOutput));
            Assert.That(docResult.FilePath, Is.EqualTo(expectedOutput));
            Assert.That(docResult.PageCount, Is.EqualTo(1));
            var input = await File.ReadAllLinesAsync(expectedOutput);
            Assert.That(input.Any(s => s.Contains("<svg")));
            Assert.That(input.Any(s => s.Contains("</svg>")));
        }
    }
}