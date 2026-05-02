using Application.Models;

namespace Application.Interfaces
{
    public interface IFileSystemManager
    {
        Task<string> ReadFileAsync(string path);
        Task WriteFileAsync(string path, string content);
        Task<ProjectUnit?> GetTreeAsync(string directoryPath);
        Task CreateDirectoryAsync(string path);
        Task DeleteAsync(string path);
        Task ChangePathAsync(string oldPath, string newName);
    }
}
