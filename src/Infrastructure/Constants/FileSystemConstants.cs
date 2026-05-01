using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Constants
{
    public class FileSystemConstants: IFileSystemConstants
    {
        public string AppName => "Schiza";
        public string ConfigFileName => "config.json";
        public string LocalFolderName => ".schiza";
    }
}
