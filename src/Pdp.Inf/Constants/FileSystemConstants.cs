using Pdp.Inf.Interfaces;

namespace Pdp.Inf.Constants
{
    public class FileSystemConstants : IFileSystemConstants
    {
        public string AppName => "Schiza";
        public string ConfigFileName => "config.json";
        public string LocalFolderName => ".schiza";
    }
}
