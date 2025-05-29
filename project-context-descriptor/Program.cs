using Ignore;
using System.Text;
using System.Text.Json;
using Ude;


class Program
{
    static Ignore.Ignore? ignore = null;
    static string? basePath = null;

    static void Main(string[] args)
    {
        basePath = Directory.GetCurrentDirectory();

        // Если расширения указаны, использовать их
        HashSet<string> extensions = new();
        if (args.Length >= 1)
        {
            extensions = args[0].Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(ext => ext.Trim().ToLowerInvariant())
                                .ToHashSet();
        }

        var ignorePath = Path.Combine(basePath, ".gitignore");
        if (File.Exists(ignorePath))
        {
            var rules = File.ReadAllLines(ignorePath);
            ignore = new Ignore.Ignore();
            foreach (var rule in rules)
                ignore.Add(rule);
        }


        var structure = BuildDirectoryStructure(basePath, basePath, extensions);
        var content = BuildFileContents(basePath, extensions);

        File.WriteAllText(Path.Combine(basePath, "project_structure.json"),
                          JsonSerializer.Serialize(structure, new JsonSerializerOptions { WriteIndented = true }));

        File.WriteAllText(Path.Combine(basePath, "project_content.txt"), content);

        Console.WriteLine("Контекст проекта сохранён.");
    }

    static bool IsIgnored(string path)
    {
        if (ignore == null) return false;

        // Получить путь относительно корня проекта
        var relative = Path.GetRelativePath(basePath ?? Directory.GetCurrentDirectory(), path).Replace("\\", "/");

        return ignore.IsIgnored(relative);
    }


    static object BuildDirectoryStructure(string rootPath, string currentPath, HashSet<string> extensions)
    {
        var result = new Dictionary<string, object>();

        var files = Directory.EnumerateFiles(currentPath)
            .Where(f => ShouldInclude(f, extensions) && !IsIgnored(f))
            .Select(Path.GetFileName)
            .ToList();

        if (files.Count > 0)
            result["."] = files;

        foreach (var dir in Directory.EnumerateDirectories(currentPath))
        {
            var subStructure = BuildDirectoryStructure(rootPath, dir, extensions);
            if (subStructure is Dictionary<string, object> dict && dict.Count > 0)
            {
                string key = Path.GetRelativePath(rootPath, dir);
                result[key] = subStructure;
            }
        }

        return result;
    }

    static string BuildFileContents(string rootPath, HashSet<string> extensions)
    {
        var sb = new StringBuilder();

        foreach (var file in Directory.EnumerateFiles(rootPath, "*.*", SearchOption.AllDirectories)
                     .Where(f => ShouldInclude(f, extensions) && !IsIgnored(f)))
        {
            string relativePath = Path.GetRelativePath(rootPath, file);
            sb.AppendLine($"File: {relativePath}");
            sb.AppendLine("```");

            try
            {
                sb.AppendLine(ReadFileWithEncoding(file));
            }
            catch (Exception ex)
            {
                sb.AppendLine($"[Ошибка чтения файла: {ex.Message}]");
            }

            sb.AppendLine("```");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    // Проверка, стоит ли включать файл
    static bool ShouldInclude(string filePath, HashSet<string> extensions)
    {
        if (extensions.Count > 0)
        {
            return extensions.Contains(Path.GetExtension(filePath).ToLowerInvariant());
        }

        // Если расширения не заданы — проверка на "текстовость"
        try
        {
            using var stream = File.OpenRead(filePath);
            int readByte;
            int maxBytes = 512;
            int totalRead = 0;

            while ((readByte = stream.ReadByte()) != -1 && totalRead < maxBytes)
            {
                if (readByte == 0) return false; // бинарный null
                if (readByte < 7 || (readByte > 13 && readByte < 32)) return false;
                totalRead++;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    static string ReadFileWithEncoding(string path)
    {
        try
        {
            // 1. Сначала проверка BOM
            using (var fs = File.OpenRead(path))
            {
                if (fs.Length >= 3)
                {
                    byte[] bom = new byte[4];
                    fs.Read(bom, 0, 4);
                    fs.Position = 0;

                    if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                        return new StreamReader(fs, Encoding.UTF8).ReadToEnd();
                    if (bom[0] == 0xFF && bom[1] == 0xFE)
                        return new StreamReader(fs, Encoding.Unicode).ReadToEnd(); // UTF-16 LE
                    if (bom[0] == 0xFE && bom[1] == 0xFF)
                        return new StreamReader(fs, Encoding.BigEndianUnicode).ReadToEnd(); // UTF-16 BE
                }

                // 2. Попытка распознать кодировку через Ude
                var detector = new CharsetDetector();
                detector.Feed(fs);
                detector.DataEnd();

                if (detector.Charset != null)
                {
                    fs.Position = 0;
                    Encoding encoding = Encoding.GetEncoding(detector.Charset);
                    using var reader = new StreamReader(fs, encoding);
                    return reader.ReadToEnd();
                }

                // 3. Если не удалось — читаем как UTF-8 без BOM
                fs.Position = 0;
                return new StreamReader(fs, new UTF8Encoding(false)).ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            return $"[Ошибка определения кодировки: {ex.Message}]";
        }
    }

}
