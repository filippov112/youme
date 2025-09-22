using System.Text;

namespace Youme.Services;

public static class StorageService
{
    // - ###input### - вводная часть
    // - ###project### - структура проекта
    // - ###settings### - пользовательские настройки                                  
    // - ###request### - запрос пользователя
    // - ###path### - относительный путь к файлу
    // - ###content### - содержание файла

    public const string KEY_WORD_INPUT = "###input###";
    public const string KEY_WORD_PROJECT = "###project###";
    public const string KEY_WORD_SETTINGS = "###settings###";
    public const string KEY_WORD_REQUEST = "###request###";
    public const string KEY_WORD_PATH = "###path###";
    public const string KEY_WORD_CONTENT = "###content###";

    public static string InputProjectPrompt { get; set; } = "Помоги мне пожалуйста со следующим проектом. Вот его структура:";
    public static string StructurePromptLocal { get; set; } = string.Empty;
    public static string StructurePromptGlobal { get; set; } = $"{KEY_WORD_INPUT}\n`````\n{KEY_WORD_PROJECT}\n`````\n{KEY_WORD_REQUEST}\n\n{KEY_WORD_SETTINGS}";
    public static string UserSettingsPrompt { get; set; } = "При ответе будь краток и лаконичен.";
    public static string StyleFileBlock { get; set; } = $"Файл: {KEY_WORD_PATH}\n````\n{KEY_WORD_CONTENT}\n````\n";


    // Методы обработки текста

    /// <summary>
    /// Финальное оформление промпта с учетом структуры проекта, пользовательских настроек и запроса пользователя
    /// </summary>
    /// <param name="projectStructure">Содержание проекта</param>
    /// <param name="userRequest">Пользовательский запрос</param>
    /// <returns></returns>
    public static string GetPrompt(string projectStructure, string userRequest)
    {
        // Если не задан локальный промпт структуры, используем глобальный
        string result = string.IsNullOrWhiteSpace(StructurePromptLocal) ? StructurePromptGlobal : StructurePromptLocal;
        
        result = result.Replace(KEY_WORD_INPUT, InputProjectPrompt);
        result = result.Replace(KEY_WORD_PROJECT, projectStructure);
        result = result.Replace(KEY_WORD_SETTINGS, UserSettingsPrompt);
        result = result.Replace(KEY_WORD_REQUEST, userRequest);
        return result;
    }

    /// <summary>
    /// Оформление блока файла для передачи в промпт
    /// </summary>
    /// <param name="relativePath">Путь к файлу</param>
    /// <param name="fileContent">Содержание</param>
    /// <returns></returns>
    public static void AddFile(StringBuilder sb, string relativePath, string fileContent)
    {
        sb.AppendLine(
            StyleFileBlock
            .Replace(KEY_WORD_PATH, relativePath)
            .Replace(KEY_WORD_CONTENT, fileContent)
        );
    }

    public static string ProjectFolder { get; set; } = string.Empty;
}