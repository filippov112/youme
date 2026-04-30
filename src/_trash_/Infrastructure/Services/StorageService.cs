using Application.Constants;
using Application.Models;
using Application.Services;
using System.Text;

namespace Infrastructure.Services;

public class StorageService: IStorageService
{
    private readonly ConfigService cs;
    public StorageService()
    {
        cs = new(SettingConstants.GlobalConfigPath, SettingConstants.LocalConfigFileName, SettingConstants.LocalConfigFolder);
        cs.LoadGlobalConfig();
    }

    public LocalConfig LConfig => cs.LC;
    public GlobalConfig GConfig => cs.GC;

    public void SaveSettings(GlobalConfig globalConfig, LocalConfig localConfig)
    {
        cs.LC = localConfig;
        cs.GC = globalConfig;
        cs.SaveGlobalConfig();
        if (!string.IsNullOrEmpty(ProjectFolder))
            cs.SaveLocalConfig(ProjectFolder);
    }

    public string GetPrompt(string projectStructure, string userRequest)
    {
        string result = string.IsNullOrWhiteSpace(cs.LC.StructurePromptLocal) ? cs.GC.StructurePromptGlobal : cs.LC.StructurePromptLocal;

        result = result.Replace(SettingConstants.KEY_WORD_INPUT, cs.LC.InputProjectPrompt);
        result = result.Replace(SettingConstants.KEY_WORD_PROJECT, projectStructure);
        result = result.Replace(SettingConstants.KEY_WORD_SETTINGS, cs.GC.UserSettingsPrompt);
        result = result.Replace(SettingConstants.KEY_WORD_REQUEST, userRequest);
        return result;
    }

    public void AddFile(StringBuilder sb, string relativePath, string fileContent)
    {
        sb.AppendLine(
            cs.GC.StyleFileBlock
            .Replace(SettingConstants.KEY_WORD_PATH, relativePath)
            .Replace(SettingConstants.KEY_WORD_CONTENT, fileContent)
        );
    }

    private string _projectFolder = string.Empty;
    public string ProjectFolder
    {
        get => _projectFolder;
        set
        {
            _projectFolder = value;
            cs.LoadLocalConfig(_projectFolder);
        }
    }
}