using System.IO;
using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared.StreamProvider
{
    public static class CrossCallingStreamProvider
    {
        private static ICustomStreamProvider _streamProvider;

        public static void RegisterStreamProvider(ICustomStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
        }

        public static async Task<Stream> OpenAsync(string filePath)
        {
            return await _streamProvider.OpenAsync(filePath);
        }

    }
}