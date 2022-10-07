using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Security.Cryptography;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using DocumentConverter.Plugin.Shared;
using DocumentConverter.Plugin.Shared.StreamProvider;
using Svg;

namespace DocumentConverter.Benchmark
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class PdfToSvgBenchmarking
    {

        public readonly IDocumentConverterService _documentConverterService;

        public PdfToSvgBenchmarking()
        {
            SvgPlatform.Init();
            _documentConverterService = new DocumentConverterService(new StreamProviderImplementation());
        }

        [Benchmark]
        public async Task ConvertPdfToSvgStreams()
        {
            await using var inputStream = File.Open(".\\Resources\\Vienna.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var outputStream = new MemoryStream();
            await _documentConverterService.ConvertPdfToSvgAsync(inputStream, outputStream);
        }

        [Benchmark]
        public async Task ConvertPdfToSvgString()
        {
            await _documentConverterService.ConvertPdfToSvgStringAsync(".\\Resources\\Vienna.pdf");
        }

        [Benchmark]
        public async Task ConvertPdfToSvgStreamsAndCreateSvgDoc()
        {
            await using var inputStream = File.Open(".\\Resources\\Vienna.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var outputStream = new MemoryStream();
            await _documentConverterService.ConvertPdfToSvgAsync(inputStream, outputStream);
            outputStream.Seek(0, SeekOrigin.Begin);

            var svgDoc = SvgDocument.Open<SvgDocument>(outputStream);
        }

        [Benchmark]
        public async Task ConvertPdfToSvgStringAndCreateSvgDoc()
        {
            var svgString =await _documentConverterService.ConvertPdfToSvgStringAsync(".\\Resources\\Vienna.pdf");
            var svgDoc = SvgDocument.FromSvg<SvgDocument>(svgString.Content);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PdfToSvgBenchmarking>();
        }
    }
}