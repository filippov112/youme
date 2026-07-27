namespace Inf.Constants
{
    public interface IFileSystemConstants
    {
        public string AppName { get; }
        public string ConfigFileName { get; }
        public string LocalFolderName { get; }
        public string SelectionsFileName { get; }
    }
    public class SystemConstants : IFileSystemConstants
    {
        public string AppName => "ProjectStudio";
        public string ConfigFileName => "config.json";
        public string LocalFolderName => ".prjs";
        public string SelectionsFileName => "selections.json";
    }
}
