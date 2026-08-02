using Core.DocsManager.Services;
using Core.Settings.Services;
using Inf.FileSystem;

namespace Inf.DocsManager
{
    public class DocsService(IConfigService config, IFileSystemWrapper fs) : IDocsService
    {
        public async Task PasteDocsCatalog()
        {
            var settings = await config.GetConfigAsync();
            var templatePath = settings.Global.DocsTemplate;

            if (string.IsNullOrEmpty(templatePath))
                throw new IOException("Не указан катало-шаблон!");
            if (!fs.DirectoryExist(templatePath))
                throw new IOException("Каталог-шаблон ненайден!");
            if (!config.ProjectOpened)
                return;

            var docsPath = Path.Combine(config.RootDirectory, Path.GetFileName(templatePath) ?? "docs");

            if (fs.DirectoryExist(docsPath))
                throw new IOException("Каталог с таким именем уже существует!");

            CopyDirectoryAll(templatePath, docsPath);
        }

        public static void CopyDirectoryAll(string sourceDir, string destinationDir, bool overwrite = true)
        {
            Directory.CreateDirectory(destinationDir);

            // Получаем все файлы из исходной директории и всех поддиректорий
            string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Вычисляем относительный путь файла
                string relativePath = Path.GetRelativePath(sourceDir, file);
                string destinationFile = Path.Combine(destinationDir, relativePath);

                // Создаем целевую директорию для файла
                string destDir = Path.GetDirectoryName(destinationFile);
                Directory.CreateDirectory(destDir);

                // Копируем файл
                File.Copy(file, destinationFile, overwrite);
            }
        }
    }
}
