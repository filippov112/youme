using Application.Constants;
using Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public interface IStorageService
    {
        public LocalConfig LConfig { get; }
        public GlobalConfig GConfig { get; }

        public void SaveSettings(GlobalConfig globalConfig, LocalConfig localConfig);

        public string GetPrompt(string projectStructure, string userRequest);

        public void AddFile(StringBuilder sb, string relativePath, string fileContent);

        public string ProjectFolder { get; set; }
    }
}
