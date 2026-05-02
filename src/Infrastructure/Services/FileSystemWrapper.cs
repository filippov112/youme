using Infrastructure.Interfaces;
using System.Text;
using Ude;

namespace Infrastructure.Services
{
    public class FileSystemWrapper(IFileSystemConstants constants) : IFileSystemWrapper
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DirectoryDelete(string path)
        {
            Directory.Delete(path, true);
        }

        public bool DirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public void DirectoryMove(string from, string to)
        {
            Directory.Move(from, to);
        }

        public async Task FileCreateAsync(string path)
        {
            using var fs = File.Create(path);
            await fs.DisposeAsync();
        }

        public void FileDelete(string path)
        {
            File.Delete(path);
        }

        public bool FileExist(string path)
        {
            return File.Exists(path);
        }

        public void FileMove(string from, string to)
        {
            File.Move(from, to);
        }

        public async Task<string> FileReadAsync(string path)
        {
            try
            {
                using var fs = File.OpenRead(path);

                // Проверка BOM
                if (fs.Length >= 3)
                {
                    byte[] bom = new byte[4];
                    fs.ReadExactly(bom, 0, 4);
                    fs.Position = 0;

                    if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                        return await new StreamReader(fs, Encoding.UTF8).ReadToEndAsync();
                    if (bom[0] == 0xFF && bom[1] == 0xFE)
                        return await new StreamReader(fs, Encoding.Unicode).ReadToEndAsync(); // UTF-16 LE
                    if (bom[0] == 0xFE && bom[1] == 0xFF)
                        return await new StreamReader(fs, Encoding.BigEndianUnicode).ReadToEndAsync(); // UTF-16 BE
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
                    return await reader.ReadToEndAsync();
                }

                // По умолчанию — UTF-8 без BOM
                fs.Position = 0;
                return await new StreamReader(fs, new UTF8Encoding(false)).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка определения кодировки: {ex.Message}");
            }
        }

        public async Task FileWriteAsync(string path, string content)
        {
            await File.WriteAllTextAsync(path, content);
        }

        public string? GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string PathCombine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        /// Можно ли парсить конкретный файл
        /// </summary>
        /// <param name="fullpath">Абсолютный путь к файлу</param>
        /// <returns>Разрешение на парсинг</returns>
        public bool FileIsReadable(string fullpath)
        {
            try
            {
                using var stream = File.OpenRead(fullpath);
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

        public string GetApplicationDirectory()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, constants.AppName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }

        public bool IsChildPath(string parent, string child)
        {
            var parentInfo = new DirectoryInfo(parent);
            var childInfo = new DirectoryInfo(child);

            while (childInfo.Parent != null)
            {
                if (childInfo.Parent.FullName.Equals(parentInfo.FullName, StringComparison.OrdinalIgnoreCase))
                    return true;

                childInfo = childInfo.Parent;
            }

            return false;
        }
    }
}
