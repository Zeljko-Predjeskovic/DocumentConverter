using System.IO;
using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared.StreamProvider
{
    public interface ICustomStreamProvider
    {
        Task<Stream> OpenAsync(string filePath);
    }
}