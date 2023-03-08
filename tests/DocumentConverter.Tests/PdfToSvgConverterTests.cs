using Shouldly;
using Svg;

namespace DocumentConverter.Tests
{
    public class PdfToSvgConverterTests
    {

        private IPdfToSvgConverter _converter;

        [SetUp]
        public void Setup()
        {
            SvgPlatform.Init();

            _converter = new PdfToSvgConverter();
        }

        [Test]
        public async Task DoesNotDisposeStreamWhenConverting()
        {
            // Arrange
            using var input = new TestableStream();
            using var output = new TestableStream();
            var filePath = ".\\Resources\\Vienna.pdf";
            var file = File.OpenRead(filePath);
            file.CopyTo(input);

            // Act
            await _converter.ConvertPdfToSvgAsync(input, output, CancellationToken.None);

            // Assert
            input.IsDisposed.ShouldBeFalse();
            output.IsDisposed.ShouldBeFalse();
        }

        private class TestableStream : MemoryStream
        {
            public bool IsDisposed { get; set; }
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                IsDisposed = true;
            }
        }
    }
}
