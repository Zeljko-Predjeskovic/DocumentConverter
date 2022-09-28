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
    }
}