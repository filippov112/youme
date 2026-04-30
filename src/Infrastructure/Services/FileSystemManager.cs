using Application.Interfaces;
using Domain.Models;
using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
    public class FileSystemManager(IFileSystemWrapper fileSystemWrapper, IDirectoryInfoWrapper factory) : IFileSystemManager
    {
        public Task<bool> FileExistsAsync(string path) => Task.FromResult(fileSystemWrapper.FileExist(path));

        public Task<bool> DirectoryExistsAsync(string path) => Task.FromResult(fileSystemWrapper.DirectoryExist(path));

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

        public async Task CreateFileAsync(string path)
        {
            var directory = fileSystemWrapper.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !fileSystemWrapper.DirectoryExist(directory))
                fileSystemWrapper.CreateDirectory(directory);

            await fileSystemWrapper.FileCreateAsync(path);
        }

        public Task CreateDirectoryAsync(string path)
        {
            fileSystemWrapper.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public Task DeleteFileAsync(string path)
        {
            fileSystemWrapper.FileDelete(path);
            return Task.CompletedTask;
        }

        public Task DeleteDirectoryAsync(string path)
        {
            fileSystemWrapper.DirectoryDelete(path);
            return Task.CompletedTask;
        }

        public Task MoveFileAsync(string sourcePath, string destinationPath)
        {
            fileSystemWrapper.FileMove(sourcePath, destinationPath);
            return Task.CompletedTask;
        }

        public Task MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            fileSystemWrapper.DirectoryMove(sourcePath, destinationPath);
            return Task.CompletedTask;
        }

        public async Task RenameAsync(string oldPath, string newName)
        {
            var directory = fileSystemWrapper.GetDirectoryName(oldPath);
            if (directory == null) 
                throw new ArgumentException("Invalid path", nameof(oldPath));

            var newPath = fileSystemWrapper.PathCombine(directory, newName);

            if (fileSystemWrapper.FileExist(oldPath))
                await MoveFileAsync(oldPath, newPath);
            else if (fileSystemWrapper.DirectoryExist(oldPath))
                await MoveDirectoryAsync(oldPath, newPath);
            else
                throw new FileNotFoundException($"Path not found: {oldPath}");
        }
    }
}
