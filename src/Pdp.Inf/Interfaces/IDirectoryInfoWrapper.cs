namespace Pdp.Inf.Interfaces
{
    public interface IDirectoryInfoWrapper
    {
        public IDirectoryInfoWrapper? Create(string directoryPath);
        public string Name { get; }
        public string FullName { get; }
        public IEnumerable<IDirectoryInfoWrapper> GetFileSystemInfos();
        public bool IsDirectory { get; }
    }
}
