using System.Text.Json;
using System.Text.Encodings.Web;

namespace ProjectContextDescriptor.ContextBuilder;

public static class StructureBuilder
{

    /// <summary>
    /// Загружает карту парсинга (pcd_context.json) в программу
    /// </summary>
    /// <param name="basePath">Сканируемый каталог</param>
    /// <returns>Словарь файлов для парсинга</returns>
    public static Dictionary<string, object>? LoadCustom(string basePath)
    {
        string pcdPath = Path.Combine(basePath, "pcd_context.json");
        if (!File.Exists(pcdPath)) 
            return null;

        try
        {
            var str = File.ReadAllText(pcdPath);
            var objStructure = JsonSerializer.Deserialize<JsonElement>(str);

            if (objStructure.ValueKind != JsonValueKind.Object)
            {
                Console.WriteLine("[Формат pcd_context.json некорректен]");
                return null;
            }

            return ConvertToDictionary(objStructure);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Ошибка чтения pcd_context.json: {ex.Message}]");
            return null;
        }
    }

    /// <summary>
    /// Рекурсивно раскладывает json-объект карты парсинга в словарь файлов
    /// </summary>
    /// <param name="element">Карта парсинга (json-объект)</param>
    /// <returns>Словарь файлов</returns>
    private static Dictionary<string, object> ConvertToDictionary(JsonElement element)
    {
        var result = new Dictionary<string, object>();

        foreach (var prop in element.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.Array) // Список имен файлов
            {
                var list = prop.Value.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String)
                    .Select(e => e.GetString()!)
                    .ToList();

                result[prop.Name] = list; // key = ".", val = [filename1, filename2, ...]
            }
            else if (prop.Value.ValueKind == JsonValueKind.Object) // Вложенный подкаталог
            {
                result[prop.Name] = ConvertToDictionary(prop.Value); // key = "fullpath/", val = { ".": [filename11, filename22, ...], ... }
            }
        }

        return result;
    }

    /// <summary>
    /// Рекурсивная функция построения дерева проекта (для словаря парсинга, либо для project_structure.json)
    /// </summary>
    /// <param name="rootPath">Корневой каталог</param>
    /// <param name="currentPath">Текущий подкаталог (относительный путь по отношению к корневому каталогу)</param>
    /// <param name="extensions">Допустимые расширения файлов</param>
    /// <param name="customStructure">Словарь парсинга (для ограничения сканирования)</param>
    /// <returns>Дерево проекта (словарь словарей файлов)</returns>
    public static Dictionary<string, object> Build(
        string rootPath,
        string currentPath,
        HashSet<string> extensions,
        Dictionary<string, object>? customStructure = null)
    {
        var result = new Dictionary<string, object>();

        // По карте парсинга
        if (customStructure != null)
        {
            foreach (var kv in customStructure)
            {
                if (kv.Key == ".")
                {
                    if (kv.Value is List<string> files1)
                    {
                        result["."] = files1
                            .Where(file => EncodingHelper.ShouldInclude(rootPath, currentPath, file, extensions))
                            .ToList();
                    }
                }
                else if (kv.Value is Dictionary<string, object> nested)
                {
                    var subdirPath = kv.Key;
                    var substructure = Build(rootPath, subdirPath, extensions, nested);
                    result[kv.Key] = substructure;
                }
            }

            return result;
        }

        // Все подряд
        var files2 = Directory.EnumerateFiles(currentPath)
            .Where(f => EncodingHelper.ShouldInclude(rootPath, currentPath, Path.GetFileName(f), extensions))
            .Select(Path.GetFileName)
            .ToList();

        if (files2.Count > 0)
            result["."] = files2;

        foreach (var dir in Directory.EnumerateDirectories(currentPath))
        {
            var substructure = Build(rootPath, dir, extensions);
            if (substructure is Dictionary<string, object> dict && dict.Count > 0)
            {
                string key = Path.GetRelativePath(rootPath, dir);
                result[key] = substructure;
            }
        }

        return result;
    }

    /// <summary>
    /// Превращает объект в строку
    /// </summary>
    /// <param name="structure"></param>
    /// <returns></returns>
    public static string Serialize(object structure)
    {
        return JsonSerializer.Serialize(structure, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
}
