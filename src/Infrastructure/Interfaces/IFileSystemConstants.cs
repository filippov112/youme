using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Interfaces
{
    public interface IFileSystemConstants
    {
        public string AppName { get; }
        public string ConfigFileName { get; }
        public string LocalFolderName { get; }
    }
}
