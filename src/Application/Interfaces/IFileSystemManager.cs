using Application.Models;

namespace Application.Interfaces
{
    public interface IFileSystemManager
    {
        Task<bool> FileExistsAsync(string path);
        Task<bool> DirectoryExistsAsync(string path);
        Task<string> ReadFileAsync(string path);
        Task WriteFileAsync(string path, string content);
        Task<ProjectUnit?> GetTreeAsync(string directoryPath);
        Task CreateFileAsync(string path);
        Task CreateDirectoryAsync(string path);
        Task DeleteFileAsync(string path);
        Task DeleteDirectoryAsync(string path);
        Task MoveFileAsync(string sourcePath, string destinationPath);
        Task MoveDirectoryAsync(string sourcePath, string destinationPath);
        Task RenameAsync(string oldPath, string newName);
    }
}
