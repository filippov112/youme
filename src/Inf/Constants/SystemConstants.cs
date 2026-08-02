namespace Inf.Constants
{
    public interface IFileSystemConstants
    {
        public string AppName { get; }
        public string ConfigFileName { get; }
        public string LocalFolderName { get; }
        public string SelectionsFileName { get; }
        public string LayoutsFileName { get; }
        public string ComponentsFileName { get; }
    }
    public class SystemConstants : IFileSystemConstants
    {
        public string AppName => "ProjectStudio";
        public string ConfigFileName => "config.json";
        public string LocalFolderName => ".prjs";
        public string SelectionsFileName => "selections.json";
        public string LayoutsFileName => "layouts.json";
        public string ComponentsFileName => "components.json";
    }
}
