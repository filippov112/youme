using System.Text.Json;
using System.Text.Encodings.Web;

namespace ProjectContextDescriptor.ContextBuilder;

public static class StructureBuilder
{
    public static Dictionary<string, object>? LoadCustom(string basePath)
    {
        string pcdPath = Path.Combine(basePath, "pcd_context.json");
        if (!File.Exists(pcdPath)) return null;

        try
        {
            var json = File.ReadAllText(pcdPath);
            var element = JsonSerializer.Deserialize<JsonElement>(json);

            if (element.ValueKind != JsonValueKind.Object)
            {
                Console.WriteLine("[Формат pcd_context.json некорректен]");
                return null;
            }

            return ConvertToDictionary(element);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Ошибка чтения pcd_context.json: {ex.Message}]");
            return null;
        }
    }

    private static Dictionary<string, object> ConvertToDictionary(JsonElement element)
    {
        var result = new Dictionary<string, object>();

        foreach (var prop in element.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.Array)
            {
                var list = prop.Value.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String)
                    .Select(e => e.GetString()!)
                    .ToList();

                result[prop.Name] = list;
            }
            else if (prop.Value.ValueKind == JsonValueKind.Object)
            {
                result[prop.Name] = ConvertToDictionary(prop.Value);
            }
        }

        return result;
    }

    public static object Build(
    string rootPath,
    string currentPath,
    HashSet<string> extensions,
    Dictionary<string, object>? customStructure = null)
    {
        var result = new Dictionary<string, object>();

        if (customStructure != null)
        {
            foreach (var kv in customStructure)
            {
                if (kv.Key == ".")
                {
                    if (kv.Value is List<string> files1)
                    {
                        result["."] = files1
                            .Where(file => EncodingHelper.ShouldInclude(Path.Combine(rootPath, currentPath, file), extensions))
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

        // Обычный режим
        var files2 = Directory.EnumerateFiles(currentPath)
            .Where(f => EncodingHelper.ShouldInclude(f, extensions))
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


    public static string Serialize(object structure)
    {
        return JsonSerializer.Serialize(structure, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
}
