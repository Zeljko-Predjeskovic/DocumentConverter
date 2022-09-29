using System.IO;
using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared.StreamProvider
{
    public static class CrossCallingStreamProvider
    {
        private static ICustomStreamProvider _streamProvider;
        public static ICustomStreamProvider StreamProvider() => _streamProvider; 
        public static void RegisterStreamProvider(ICustomStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
        }
    }
}