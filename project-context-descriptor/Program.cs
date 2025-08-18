using System.Text;
using ProjectContextDescriptor.ContextBuilder;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var basePath = Directory.GetCurrentDirectory();
HashSet<string>? extensions = new HashSet<string>();

// Расширения
if (args.Length > 0 && args[^1] != "struct")
    extensions = [.. args[^1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToLower())];

// Режим "только карта"
if (args.Length > 0 && args[0] == "struct")
{
    Dictionary<string,object> map = StructureBuilder.Build(basePath, basePath, extensions);

    SaveFile(basePath, "pcd_context.json", StructureBuilder.Serialize(map));
    return;
}
// Режим парсинга
else
{
    Dictionary<string,object>? map = StructureBuilder.LoadCustom(basePath);

    Dictionary<string, object> fullStructure = StructureBuilder.Build(basePath, basePath, extensions, map);
    string content = ContentBuilder.Build(basePath, extensions, map);

    SaveFile(basePath, "project_structure.json", StructureBuilder.Serialize(fullStructure));
    SaveFile(basePath, "project_content.txt", content);
}

void SaveFile(string directory, string filename, string text)
{
    File.WriteAllText(Path.Combine(directory, filename), text);
    Console.WriteLine($"Файл {filename} сохранен!");
}
  
