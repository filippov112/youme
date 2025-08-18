using System.Text;

namespace ProjectContextDescriptor.ContextBuilder;

public static class ContentBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="extensions"></param>
    /// <param name="structure"></param>
    /// <returns></returns>
    public static string Build(
        string rootPath,
        HashSet<string> extensions,
        Dictionary<string, object>? structure = null)
    {
        var sb = new StringBuilder();

        if (structure != null)
            TraverseCustomStructure(rootPath, structure, sb, extensions);
        else
            TraverseDirectory(rootPath, sb, extensions);

        return sb.ToString();
    }

    /// <summary>
    /// Обычная функция парсинга текста со всех файлов корневого каталога, включая подкаталоги
    /// </summary>
    /// <param name="path">Корневой каталог</param>
    /// <param name="sb">Текст содержимого</param>
    /// <param name="extensions">Допустимые расширения файлов</param>
    private static void TraverseDirectory(
        string path,
        StringBuilder sb,
        HashSet<string> extensions)
    {
        // Перебираем одним циклом все файлы во всех подкаталогах
        foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                     .Where(f => EncodingHelper.ShouldInclude(f.Split(Path.GetFileName(f))[0], "", Path.GetFileName(f), extensions)))
        {
            AppendFileContent(sb, path, file);
        }
    }


    /// <summary>
    /// Рекурсивная функция парсинга содержимого файлов каталога по словарю парсинга
    /// </summary>
    /// <param name="rootPath">Сканируемый главный каталог</param>
    /// <param name="structure">Словарь парсинга</param>
    /// <param name="sb">Текст содержимого</param>
    /// <param name="extensions">Допустимые расширения</param>
    /// <param name="current">Текущий сканируемый подкаталог</param>
    private static void TraverseCustomStructure(
        string rootPath,
        Dictionary<string, object> structure,
        StringBuilder sb,
        HashSet<string> extensions,
        string current = "")
    {
        // Рекурсивно проходим по словарю парсинга
        foreach (var kv in structure)
        {
            // Список файлов
            if (kv.Key == ".")
            {
                if (kv.Value is List<string> files)
                {
                    foreach (var file in files.Where(f => EncodingHelper.ShouldInclude(rootPath, current, f, extensions)))
                    {
                        var fullPath = Path.Combine(rootPath, current, file);
                        if (File.Exists(fullPath))
                            AppendFileContent(sb, rootPath, fullPath); // Если файл из словаря существует, парсим его
                    }
                }
            }
            // Подкаталог
            else if (kv.Value is Dictionary<string, object> nested)
            {
                TraverseCustomStructure(rootPath, nested, sb, extensions, kv.Key); // kv.key = относительный путь от корневого каталога проекта
            }
        }
    }


    /// <summary>
    /// Функция добавления содержимого в конец текста
    /// </summary>
    /// <param name="sb">Текст содержимого всех файлов (StringBuilder)</param>
    /// <param name="rootPath">Абсолютный путь к сканируемому каталогу</param>
    /// <param name="file">Абсолютный путь к файлу</param>
    private static void AppendFileContent(StringBuilder sb, string rootPath, string file)
    {
        string relativePath = Path.GetRelativePath(rootPath, file);
        sb.AppendLine($"File: {relativePath}");
        sb.AppendLine("```");

        try
        {
            sb.AppendLine(EncodingHelper.ParseFile(file));
        }
        catch (Exception ex)
        {
            sb.AppendLine($"[Ошибка чтения файла: {ex.Message}]");
        }

        sb.AppendLine("```");
        sb.AppendLine();
    }
}
