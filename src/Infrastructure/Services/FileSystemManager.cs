using Application.Interfaces;
using Application.Models;
using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
    public class FileSystemManager(IFileSystemWrapper fileSystemWrapper, IDirectoryInfoWrapper factory) : IFileSystemManager
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
        public async Task<ProjectUnit?> GetTreeAsync(string rootPath)
        {
            var rootInfo = factory.Create(rootPath);
            if (!rootInfo.IsDirectory && !fileSystemWrapper.FileIsReadable(rootInfo.FullName))
                return null;
            return CreateNode(rootInfo, null);
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
        private ProjectUnit CreateNode(IDirectoryInfoWrapper info, ProjectUnit? parent)
        {
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
                    if (!childInfo.IsDirectory && !fileSystemWrapper.FileIsReadable(childInfo.FullName))
                        continue;
                    var childNode = CreateNode(childInfo, node);
                    node.Children.Add(childNode);
                }
            }
            return node;
        }
        #endregion
    }
}
