using System.IO;
using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared.StreamProvider
{
    public class StreamProviderImplementation : ICustomStreamProvider
    {
        public Task<Stream> OpenReadAsync(string filePath)
        {
            var stream = File.Open(filePath,FileMode.Open) as Stream;
            return Task.FromResult(stream);
        }

        public Task<Stream> OpenReadWriteAsync(string filePath)
        {
            var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite) as Stream;
            return Task.FromResult(stream);
        }
    }
}