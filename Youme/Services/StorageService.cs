using System.Text;
using Youme.Model;

namespace Youme.Services;

public class StorageService
{
    #region Constants
    /// <summary>
    /// Относительный маршрут настроек программы
    /// </summary>
    private string GlobalConfigPath = "./config/global_config.json";
    /// <summary>
    /// Файл настроек проекта
    /// </summary>
    private string LocalConfigFileName = "local_config.json";
    /// <summary>
    /// Каталог служебных файлов проекта
    /// </summary>
    private string LocalConfigFolder = ".youme";
    /// <summary>
    /// - ###input### - вводная часть
    /// </summary>
    public const string KEY_WORD_INPUT = "###input###";
    /// <summary>
    /// - ###project### - структура проекта
    /// </summary>
    public const string KEY_WORD_PROJECT = "###project###";
    /// <summary>
    /// - ###settings### - пользовательские настройки 
    /// </summary>
    public const string KEY_WORD_SETTINGS = "###settings###";                                 
    /// <summary>
    /// - ###request### - запрос пользователя
    /// </summary>
    public const string KEY_WORD_REQUEST = "###request###";
    /// <summary>
    /// - ###path### - относительный путь к файлу
    /// </summary>
    public const string KEY_WORD_PATH = "###path###";
    /// <summary>
    /// - ###content### - содержание файла
    /// </summary>
    public const string KEY_WORD_CONTENT = "###content###";
    #endregion

    private ConfigService cs;
    public StorageService()
    {
        cs = new(GlobalConfigPath, LocalConfigFileName, LocalConfigFolder);
    }
    public void LoadGlobalConfig()
    {
        cs.LoadGlobalConfig();
    }
    public void LoadLocalConfig(string directoryPath)
    {
        cs.LoadLocalConfig(directoryPath);
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

    
    // Методы обработки текста

    /// <summary>
    /// Финальное оформление промпта с учетом структуры проекта, пользовательских настроек и запроса пользователя
    /// </summary>
    /// <param name="projectStructure">Содержание проекта</param>
    /// <param name="userRequest">Пользовательский запрос</param>
    /// <returns></returns>
    public string GetPrompt(string projectStructure, string userRequest)
    {
        // Если не задан локальный промпт структуры, используем глобальный
        string result = string.IsNullOrWhiteSpace(cs.LC.StructurePromptLocal) ? cs.GC.StructurePromptGlobal : cs.LC.StructurePromptLocal;
        
        result = result.Replace(KEY_WORD_INPUT, cs.LC.InputProjectPrompt);
        result = result.Replace(KEY_WORD_PROJECT, projectStructure);
        result = result.Replace(KEY_WORD_SETTINGS, cs.GC.UserSettingsPrompt);
        result = result.Replace(KEY_WORD_REQUEST, userRequest);
        return result;
    }

    /// <summary>
    /// Оформление блока файла для передачи в промпт
    /// </summary>
    /// <param name="relativePath">Путь к файлу</param>
    /// <param name="fileContent">Содержание</param>
    /// <returns></returns>
    public void AddFile(StringBuilder sb, string relativePath, string fileContent)
    {
        sb.AppendLine(
            cs.GC.StyleFileBlock
            .Replace(KEY_WORD_PATH, relativePath)
            .Replace(KEY_WORD_CONTENT, fileContent)
        );
    }

    private string _projectFolder = string.Empty;
    public string ProjectFolder
    {
        get => _projectFolder;
        set
        {
            _projectFolder = value;
            LoadLocalConfig(_projectFolder);
        }
    }
}