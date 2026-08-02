using Core.CatalogParser.Models;
using Core.CatalogParser.Services;
using Core.Explorer.Models;
using Core.Explorer.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Inf.CatalogParser;

public class CatalogJsonProcessor(IFileSystemService fileSystemService) : ICatalogJsonProcessor
{


    /// <summary>
    /// Метод 1: Парсинг содержимого каталога и возврат в формате JSON
    /// </summary>
    /// <param name="directoryPath">Путь к каталогу</param>
    /// <returns>JSON-строка с иерархией каталога</returns>
    public async Task<string> ParseDirectoryToJson(string[]? includeFiles = null)
    {
        var tree = await fileSystemService.GetTreeAsync();
        if (tree == null)
            return string.Empty;

        var childItems = new List<CatalogItem>();
        foreach (var child in tree.Children)
        {
            var newItem = ParseDirectoryRecursive(includeFiles, child);
            if (newItem != null)
                childItems.Add(newItem);
        }
        var rootItem = new CatalogItem
        {
            IsDirectory = true,
            Children = childItems
        };

        // Сериализуем в нужном формате
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Converters = { new CatalogItemConverter() }
        };
        var options = jsonSerializerOptions;

        return JsonSerializer.Serialize(rootItem, options);
    }

    private static CatalogItem? ParseDirectoryRecursive(string[]? includeFiles, ProjectUnit contentList)
    {
        if (contentList.IsDirectory)
        {
            var childItems = new List<CatalogItem>();
            foreach (var child in contentList.Children)
            {
                var newItem = ParseDirectoryRecursive(includeFiles, child);
                if (newItem != null)
                    childItems.Add(newItem);
            }
            if (childItems.Count == 0)
                return null;

            return new CatalogItem
            {
                Name = contentList.Name,
                IsDirectory = true,
                Children = childItems
            };
        }

        // Если это файл
        if (includeFiles is null || includeFiles.Contains(contentList.Path))
        {
            return new CatalogItem
            {
                Name = contentList.Name,
                IsDirectory = false
            };
        }
        return null;
    }


    /// <summary>
    /// Кастомный конвертер для сериализации в нужном формате
    /// </summary>
    private class CatalogItemConverter : JsonConverter<CatalogItem>
    {
        public override CatalogItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            // Для простоты используем стандартную десериализацию
            JsonSerializer.Deserialize<CatalogItem>(ref reader, options);

        public override void Write(Utf8JsonWriter writer, CatalogItem value, JsonSerializerOptions options)
        {
            if (value.IsDirectory)
            {
                if (value.Name is null)
                    writer.WriteStartObject();
                else
                    writer.WriteStartObject(value.Name);
                foreach (var child in value.Children!)
                {
                    Write(writer, child, options);
                }
                writer.WriteEndObject();
            }
            else
            {
                // Для файлов
                writer.WriteString(value.Name, value.Description ?? "");
            }
        }
    }


    /// <summary>
    /// Метод 2: Объединение двух JSON-структур
    /// </summary>
    /// <param name="sourceJson">Исходный JSON (описания берутся из него)</param>
    /// <param name="structureJson">JSON со структурой для объединения</param>
    /// <returns>Объединенная JSON-строка</returns>
    public string MergeJsonStructures(string sourceJson, string structureJson)
    {
        // Более простая реализация, если нужно сохранить именно формат из примера
        // с корневым объектом { "root_catalog": [...] }

        var sourceDict = JsonSerializer.Deserialize<Dictionary<string, object>>(sourceJson);
        var structureDict = JsonSerializer.Deserialize<Dictionary<string, object>>(structureJson);

        if (sourceDict == null || structureDict == null)
        {
            throw new ArgumentException("Неверный формат JSON");
        }

        var result = MergeDictionaries(sourceDict, structureDict);
        JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
        JsonSerializerOptions options = jsonSerializerOptions;
        return JsonSerializer.Serialize(result, options);
    }

    private Dictionary<string, object> MergeDictionaries(Dictionary<string, object> source, Dictionary<string, object> structure)
    {
        var result = new Dictionary<string, object>();

        foreach (var kvp in structure)
        {
            if (!source.ContainsKey(kvp.Key))
            {
                result[kvp.Key] = kvp.Value; // Новый элемент
                continue;
            }

            // Рекурсивно объединяем
            if (kvp.Value is JsonElement structElem && source[kvp.Key] is JsonElement sourceElem)
            {
                if (sourceElem.ValueKind == JsonValueKind.Object && structElem.ValueKind == JsonValueKind.Object) // 2 каталога (рекурсия)
                {
                    var sourceDict = sourceElem.Deserialize<Dictionary<string, object>>();
                    var structDict = structElem.Deserialize<Dictionary<string, object>>();
                    result[kvp.Key] = MergeDictionaries(sourceDict, structDict);
                }
                else if (structElem.ValueKind != JsonValueKind.Object && sourceElem.ValueKind != JsonValueKind.Object) // 2 файла (старое описание)
                {
                    result[kvp.Key] = source[kvp.Key];
                }
                else if (structElem.ValueKind != JsonValueKind.Object) // Был каталог, стал файл (пустое описание)
                {
                    result[kvp.Key] = kvp.Value;
                }
                else // Был файл, стал каталог (рекурсия с 2 копиями)
                {
                    var sourceDict = structElem.Deserialize<Dictionary<string, object>>();
                    var structDict = structElem.Deserialize<Dictionary<string, object>>();
                    result[kvp.Key] = MergeDictionaries(sourceDict, structDict);
                }
            }
            else
                throw new IOException("Некорректный формат json-структур!");
        }

        return result;
    }
}
