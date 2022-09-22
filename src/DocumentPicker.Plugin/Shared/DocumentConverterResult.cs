using System;
using System.Linq;
using Svg;

namespace DocumentConverter.Plugin.Shared
{
    public class DocumentConverterResult
    {
        public string Content { get; set; }
        public string ResultPath { get; set; }
        public int PageCount { get; set; }

    }
}