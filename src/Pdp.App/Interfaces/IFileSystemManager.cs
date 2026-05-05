using Pdp.App.Models;

namespace Pdp.App.Interfaces
{
    public interface IFileSystemManager
    {
        Task<string> ReadFileAsync(string path);
        Task WriteFileAsync(string path, string content);
        Task<ProjectUnit?> GetTreeAsync(string pattern = "");
        Task CreateDirectoryAsync(string path);
        Task DeleteAsync(string path);
        Task ChangePathAsync(string oldPath, string newName);
    }
}
