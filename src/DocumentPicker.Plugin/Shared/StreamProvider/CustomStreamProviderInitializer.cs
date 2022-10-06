using System.IO;
using System.Threading.Tasks;
#if WINDOWS_UWP
using DocumentConverter.Plugin.Platforms.UniversalWindows;
#endif

namespace DocumentConverter.Plugin.Shared.StreamProvider
{
    public static class CustomStreamProviderInitializer
    {

        public static void Init()
        {
#if WINDOWS_UWP
            CustomStreamProvider.Instance = new UniversalWindowsStreamProvider();
#else
            CustomStreamProvider.Instance = new StreamProviderImplementation();
#endif
        }
    }
}