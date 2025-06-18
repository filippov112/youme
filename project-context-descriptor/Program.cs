using System.Text;
using ProjectContextDescriptor.ContextBuilder;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var basePath = Directory.GetCurrentDirectory();

HashSet<string>? extensions = args.Length >= 1 && args[0] != "struct"
    ? [.. args[0].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToLower())]
    : [];

if (args.Length > 0 && args[0] == "struct")
{
    extensions = args.Length >= 2 
        ? [.. args[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToLower())]
        : [];
    var structure = StructureBuilder.Build(basePath, basePath, extensions);
    File.WriteAllText(Path.Combine(basePath, "pcd_context.json"),
        StructureBuilder.Serialize(structure));
    Console.WriteLine("Файл pcd_context.json создан.");
    return;
}

var custom = StructureBuilder.LoadCustom(basePath);
var fullStructure = StructureBuilder.Build(basePath, basePath, extensions, custom);
var content = ContentBuilder.Build(basePath, extensions, custom);

File.WriteAllText(Path.Combine(basePath, "project_structure.json"),
    StructureBuilder.Serialize(fullStructure));
File.WriteAllText(Path.Combine(basePath, "project_content.txt"), content);
Console.WriteLine("Контекст проекта сохранён.");
