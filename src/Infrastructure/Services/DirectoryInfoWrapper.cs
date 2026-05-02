using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
    public class DirectoryInfoWrapper : IDirectoryInfoWrapper
    {
        private readonly FileSystemInfo _info;
        private DirectoryInfoWrapper(FileSystemInfo info)
        {
            _info = info;
            FullName = info.FullName;
            Name = info.Name;
            IsDirectory = info is DirectoryInfo;
        }
        public string FullName { get; }
        public string Name { get; }
        public bool IsDirectory { get; }

        public IEnumerable<IDirectoryInfoWrapper> GetFileSystemInfos()
        {
            if (IsDirectory)
                return ((DirectoryInfo)_info).GetFileSystemInfos().Select(fsi => new DirectoryInfoWrapper(fsi));
            return [];
        }

        public IDirectoryInfoWrapper? Create(string directoryPath)
        {
            return CreateStatic(directoryPath);
        }

        public static IDirectoryInfoWrapper? CreateStatic(string directoryPath)
        {
            if (File.Exists(directoryPath))
                return new DirectoryInfoWrapper(new FileInfo(directoryPath));
            else if (Directory.Exists(directoryPath))
                return new DirectoryInfoWrapper(new DirectoryInfo(directoryPath));
            else
                return null;
        }
    }
}
