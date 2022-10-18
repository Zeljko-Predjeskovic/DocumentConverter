using System;
using System.IO;
using DocumentConverter.Plugin.Shared.StreamProvider;

namespace DocumentConverter.Plugin.Shared.FileSharing
{
    public static class SharedStream
    {
        private static Stream _stream;

        public static Stream Instance
        {
            get => _stream;
            set => _stream = value;
        }

        public static void DisposeStream()
        {
            Instance.Dispose();
        }
    }
}