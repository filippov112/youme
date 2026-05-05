namespace Pdp.Inf.Interfaces
{
    public interface IFileSystemWrapper
    {
        public bool FileExist(string path);
        public bool DirectoryExist(string path);
        public Task<string> FileReadAsync(string path);
        public string? GetDirectoryName(string path);
        public void CreateDirectory(string path);
        public Task FileWriteAsync(string path, string content);
        public Task FileCreateAsync(string path);
        public void FileDelete(string path);
        public void DirectoryDelete(string path);
        public void FileMove(string from, string to);
        public void DirectoryMove(string from, string to);
        public string PathCombine(params string[] prms);
        public bool FileIsReadable(string fullpath);
        public string GetApplicationDirectory();
        public bool IsChildPath(string parent, string child);
    }
}