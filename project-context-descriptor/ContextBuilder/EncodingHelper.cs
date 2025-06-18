using System.Text;
using Ude;

namespace ProjectContextDescriptor.ContextBuilder;

public static class EncodingHelper
{
    public static bool ShouldInclude(string filePath, HashSet<string> extensions)
    {
        Console.WriteLine(filePath);
        // Если указаны расширения — фильтровать по ним
        if (extensions.Count > 0)
        {
            return extensions.Contains(Path.GetExtension(filePath).ToLowerInvariant());
        }

        // Иначе проверка на "текстовость" по байтам
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

    public static string ReadFileWithEncoding(string path)
    {
        try
        {
            using var fs = File.OpenRead(path);

            // Проверка BOM
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

            // Определение кодировки через Ude
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

            // По умолчанию — UTF-8 без BOM
            fs.Position = 0;
            return new StreamReader(fs, new UTF8Encoding(false)).ReadToEnd();
        }
        catch (Exception ex)
        {
            return $"[Ошибка определения кодировки: {ex.Message}]";
        }
    }
}
