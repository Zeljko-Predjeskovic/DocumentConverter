using System.IO;
using System.Threading.Tasks;

namespace DocumentConverter.Plugin.Shared.StreamProvider
{
    public interface ICustomStreamProvider
    {
        Task<Stream> OpenReadAsync(string filePath);
        Task<Stream> OpenReadWriteAsync(string filePath);
    }
}