using Core.Explorer.Models;

namespace Core.Explorer.Services
{
    public interface IFileSystemService
    {
        Task<string> ReadFileAsync(string path);
        Task WriteFileAsync(string path, string content);
        Task<ProjectUnit?> GetTreeAsync();
        Task CreateDirectoryAsync(string path);
        Task DeleteAsync(string path);
        Task ChangePathAsync(string oldPath, string newName);
    }
}
