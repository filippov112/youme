using System.Text;
using System.Text.Json;

namespace ProjectContextDescriptor.ContextBuilder;

public static class ContentBuilder
{
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

    private static void TraverseDirectory(
        string path,
        StringBuilder sb,
        HashSet<string> extensions)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                     .Where(f => EncodingHelper.ShouldInclude(f, extensions)))
        {
            AppendFileContent(sb, path, file);
        }
    }

    private static void TraverseCustomStructure(
    string rootPath,
    Dictionary<string, object> structure,
    StringBuilder sb,
    HashSet<string> extensions,
    string current = "")
    {
        foreach (var kv in structure)
        {
            if (kv.Key == ".")
            {
                if (kv.Value is List<string> files)
                {
                    foreach (var file in files
                        .Where(f => EncodingHelper.ShouldInclude(Path.Combine(rootPath, current, f), extensions)))
                    {
                        var fullPath = Path.Combine(rootPath, current, file);
                        if (File.Exists(fullPath))
                            AppendFileContent(sb, rootPath, fullPath);
                    }
                }
            }
            else if (kv.Value is Dictionary<string, object> nested)
            {
                TraverseCustomStructure(rootPath, nested, sb, extensions, kv.Key);
            }
        }
    }

    private static void AppendFileContent(StringBuilder sb, string rootPath, string file)
    {
        string relativePath = Path.GetRelativePath(rootPath, file);
        sb.AppendLine($"File: {relativePath}");
        sb.AppendLine("```");

        try
        {
            sb.AppendLine(EncodingHelper.ReadFileWithEncoding(file));
        }
        catch (Exception ex)
        {
            sb.AppendLine($"[Ошибка чтения файла: {ex.Message}]");
        }

        sb.AppendLine("```");
        sb.AppendLine();
    }
}
