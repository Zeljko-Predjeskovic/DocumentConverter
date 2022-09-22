using DocumentConverter.Plugin.Shared;
using DocumentConverter.Plugin.Shared.Picker;
using DocumentConverter.Plugin.Shared.StreamProvider;
using Moq;
using Svg;

namespace DocumentConverter.Tests;

public class DocumentConversionExtensionTests
{
    private readonly Mock<IFilePicker> _filePickerMock = new();
    private IDocumentConverterService _documentConverterService;

    [SetUp]
    public void Setup()
    {
        var streamProvider = new StreamProviderImplementation();
        CrossCallingStreamProvider.RegisterStreamProvider(streamProvider);
        _documentConverterService = new DocumentConverterService(_filePickerMock.Object, streamProvider);
        SvgPlatform.Init();
    }

    [Test]
    public async Task WhenGivingFilePath_GetPdfInfo()
    {
        //Arrange
        var filePath = ".\\Resources\\l1.pdf";

        //Act
        var pdfInfo = await DocumentConversionExtension.GetPdfFileInfo(filePath);

        //Assert
        Assert.That(pdfInfo.PageCount, Is.EqualTo(4));
        Assert.That(pdfInfo.Author, Is.EqualTo("BMBWF"));
        Assert.That(pdfInfo.Title, Is.EqualTo(null));
        Assert.That(pdfInfo.FilePath, Is.EqualTo(filePath));
    }

    [Test]
    public void WhenGivingNotPdfFilePath_GetPdfInfo()
    {
        //Arrange
        var filePath = ".\\Resources\\TextFile.txt";

        //Act
        var getPdfInfo =  DocumentConversionExtension.GetPdfFileInfo(filePath);

        //Assert
        Assert.ThrowsAsync<DocumentConverterException>(async () => await getPdfInfo, "File is not a Pdf!");
    }

    [Test]
    public async Task WhenPdfIsScannedImage_ThenSvgStringShouldBeBase64()
    {
        //Arrange
        var filePath = ".\\Resources\\Scanned.pdf";
        var outputDir = ".\\Resources";

        //Act
        string svgSring = await _documentConverterService.ConvertPdfToSvgStringAsync(filePath);
        var svgDoc = SvgDocument.FromSvg<SvgDocument>(svgSring);

        //Assert
        Assert.True(DocumentConversionExtension.SvgHasOnlyImages(svgDoc));
    }

    [Test]
    public async Task WhenPdfIsDrawnVector_ThenSvgStringShouldNotBeBase64()
    {
        //Arrange
        var filePath = ".\\Resources\\l1.pdf";
        var outputDir = ".\\Resources";

        //Act
        var docResult = await _documentConverterService.ConvertPdfToSvgAsync(filePath, outputDir);
        var svgDoc = SvgDocument.FromSvg<SvgDocument>(await File.ReadAllTextAsync(docResult.FilePath));

        //Assert
        Assert.False(DocumentConversionExtension.SvgHasOnlyImages(svgDoc));
    }
}