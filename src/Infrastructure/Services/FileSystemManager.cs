using Application.Interfaces;
using Application.Models;
using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
    public class FileSystemManager(IFileSystemWrapper fileSystemWrapper, IDirectoryInfoWrapper factory, IConfigService config) : IFileSystemManager
    {
        public async Task<string> ReadFileAsync(string path)
        {
            return await fileSystemWrapper.FileReadAsync(path);
        }
        public async Task WriteFileAsync(string path, string content)
        {
            var directory = fileSystemWrapper.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !fileSystemWrapper.DirectoryExist(directory))
                fileSystemWrapper.CreateDirectory(directory);

            await fileSystemWrapper.FileWriteAsync(path, content);
        }
        public async Task<ProjectUnit?> GetTreeAsync(string pattern = "")
        {
            var rootInfo = factory.Create(config.RootDirectory);
            if (rootInfo == null)
                return null;
            return await CreateNode(rootInfo, null, pattern);
        }
        public Task CreateDirectoryAsync(string path)
        {
            fileSystemWrapper.CreateDirectory(path);
            return Task.CompletedTask;
        }
        public async Task DeleteAsync(string path)
        {
            if (fileSystemWrapper.FileExist(path))
                await DeleteFileAsync(path);
            else if (fileSystemWrapper.DirectoryExist(path))
                await DeleteDirectoryAsync(path);
            else
                throw new FileNotFoundException($"Path not found: {path}");
        }
        public async Task ChangePathAsync(string oldPath, string newName)
        {
            var directory = fileSystemWrapper.GetDirectoryName(oldPath) ??
                throw new ArgumentException("Invalid path", nameof(oldPath));
            var newPath = fileSystemWrapper.PathCombine(directory, newName);

            if (fileSystemWrapper.IsChildPath(oldPath, newPath))
                throw new ArgumentException("Invalid path", nameof(newPath));

            if (fileSystemWrapper.FileExist(oldPath))
                await MoveFileAsync(oldPath, newPath);
            else if (fileSystemWrapper.DirectoryExist(oldPath))
                await MoveDirectoryAsync(oldPath, newPath);
            else
                throw new FileNotFoundException($"Path not found: {oldPath}");
        }
        #region Private
        private Task DeleteFileAsync(string path)
        {
            fileSystemWrapper.FileDelete(path);
            return Task.CompletedTask;
        }
        private Task DeleteDirectoryAsync(string path)
        {
            fileSystemWrapper.DirectoryDelete(path);
            return Task.CompletedTask;
        }
        private Task MoveFileAsync(string sourcePath, string destinationPath)
        {
            fileSystemWrapper.FileMove(sourcePath, destinationPath);
            return Task.CompletedTask;
        }
        private Task MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            fileSystemWrapper.DirectoryMove(sourcePath, destinationPath);
            return Task.CompletedTask;
        }
        private async Task<ProjectUnit?> CreateNode(IDirectoryInfoWrapper info, ProjectUnit? parent, string pattern)
        {
            if (!info.IsDirectory && !fileSystemWrapper.FileIsReadable(info.FullName))
                return null;
            if (!info.IsDirectory && !string.IsNullOrEmpty(pattern) && !(await ReadFileAsync(info.FullName)).Contains(pattern))
                return null;

            string path = info.FullName;
            var node = new ProjectUnit
            {
                Name = info.Name,
                Path = path,
                Parent = parent,
                IsDirectory = info.IsDirectory
            };
            if (info.IsDirectory)
            {
                foreach (var childInfo in info.GetFileSystemInfos())
                {
                    var childNode = await CreateNode(childInfo, node, pattern);
                    if (childNode != null)
                        node.Children.Add(childNode);
                }
            }
            if (info.IsDirectory && node.Children.Count == 0 && !string.IsNullOrEmpty(pattern))
                return null;
            return node;
        }
        #endregion
    }
}
