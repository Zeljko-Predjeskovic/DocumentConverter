using System.IO;
using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared.StreamProvider
{
    public class StreamProviderImplementation : ICustomStreamProvider
    {
        public async Task<Stream> OpenAsync(string filePath)
        {
            var stream = await Task.FromResult(File.Open(filePath,FileMode.Open));
            return stream;
        }
    }
}