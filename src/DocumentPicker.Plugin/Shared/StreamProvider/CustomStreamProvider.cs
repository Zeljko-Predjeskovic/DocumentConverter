
namespace DocumentConverter.Plugin.Shared.StreamProvider
{
    public static class CustomStreamProvider
    {
        private static ICustomStreamProvider _customStreamProvider;

        public static ICustomStreamProvider Instance
        {
            get => _customStreamProvider;
            set => _customStreamProvider = value;
        }
    }
}