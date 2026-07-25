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



        var rootItem = ParseDirectoryRecursive(includeFiles, tree);

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
    /// Метод 2: Объединение двух JSON-структур
    /// </summary>
    /// <param name="sourceJson">Исходный JSON (описания берутся из него)</param>
    /// <param name="structureJson">JSON со структурой для объединения</param>
    /// <returns>Объединенная JSON-строка</returns>
    public string MergeJsonStructures(string sourceJson, string structureJson)
    {
        var sourceItem = JsonSerializer.Deserialize<CatalogItem>(sourceJson);
        var structureItem = JsonSerializer.Deserialize<CatalogItem>(structureJson);

        if (sourceItem == null || structureItem == null)
        {
            throw new ArgumentException("Неверный формат JSON");
        }

        var result = MergeItems(sourceItem, structureItem);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new CatalogItemConverter() }
        };

        return JsonSerializer.Serialize(result, options);
    }

    private CatalogItem MergeItems(CatalogItem source, CatalogItem structure)
    {
        var result = new CatalogItem
        {
            Name = source.Name,
            Description = source.Description,
            IsDirectory = source.IsDirectory
        };

        if (source.Children == null || structure.Children == null)
        {
            result.Children = source.Children ?? structure.Children;
            return result;
        }

        // Создаем словарь для быстрого поиска по имени
        var sourceDict = source.Children.ToDictionary(x => x.Name);
        var structureDict = structure.Children.ToDictionary(x => x.Name);

        result.Children = new List<CatalogItem>();

        // Проходим по структуре (берем порядок из structure)
        foreach (var structItem in structure.Children)
        {
            if (sourceDict.TryGetValue(structItem.Name, out var sourceItem))
            {
                // Если элемент есть в обоих - рекурсивно объединяем
                if (sourceItem.IsDirectory && structItem.IsDirectory)
                {
                    result.Children.Add(MergeItems(sourceItem, structItem));
                }
                else
                {
                    // Для файлов берем описание из source
                    result.Children.Add(sourceItem);
                }
            }
            else
            {
                // Если элемента нет в source - берем как есть из structure
                result.Children.Add(structItem);
            }
        }

        // Добавляем элементы из source, которых нет в structure
        foreach (var sourceItem in source.Children)
        {
            if (!structureDict.ContainsKey(sourceItem.Name))
            {
                result.Children.Add(sourceItem);
            }
        }

        // Сортируем для стабильности результата
        result.Children = result.Children.OrderBy(x => x.IsDirectory ? 0 : 1)
                                        .ThenBy(x => x.Name)
                                        .ToList();

        return result;
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
            if (value.IsDirectory && value.Children != null)
            {
                writer.WriteStartObject();

                if (!string.IsNullOrEmpty(value.Description))
                {
                    writer.WriteString(value.Name, value.Description);
                }
                else
                {
                    writer.WriteStartArray(value.Name);
                    foreach (var child in value.Children)
                    {
                        Write(writer, child, options);
                    }
                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
            }
            else
            {
                // Для файлов
                writer.WriteStartObject();
                writer.WriteString(value.Name, value.Description ?? "");
                writer.WriteEndObject();
            }
        }
    }

    /// <summary>
    /// Альтернативная реализация для формата, где у каталога всегда массив
    /// </summary>
    public string MergeJsonStructuresSimple(string sourceJson, string structureJson)
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
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    private Dictionary<string, object> MergeDictionaries(Dictionary<string, object> source, Dictionary<string, object> structure)
    {
        var result = new Dictionary<string, object>();

        foreach (var kvp in structure)
        {
            if (source.ContainsKey(kvp.Key))
            {
                // Рекурсивно объединяем
                if (kvp.Value is JsonElement sourceElem && source[kvp.Key] is JsonElement structElem)
                {
                    if (sourceElem.ValueKind == JsonValueKind.Object && structElem.ValueKind == JsonValueKind.Object)
                    {
                        var sourceDict = sourceElem.Deserialize<Dictionary<string, object>>();
                        var structDict = structElem.Deserialize<Dictionary<string, object>>();
                        result[kvp.Key] = MergeDictionaries(sourceDict, structDict);
                    }
                    else
                    {
                        result[kvp.Key] = source[kvp.Key];
                    }
                }
                else
                {
                    result[kvp.Key] = source[kvp.Key];
                }
            }
            else
            {
                result[kvp.Key] = kvp.Value;
            }
        }

        // Добавляем элементы из source, которых нет в structure
        foreach (var kvp in source)
        {
            if (!structure.ContainsKey(kvp.Key))
            {
                result[kvp.Key] = kvp.Value;
            }
        }

        return result;
    }
}
