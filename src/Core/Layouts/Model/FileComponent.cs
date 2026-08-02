using Core.Layouts.DTO;

namespace Core.Layouts.Model
{
    /// <summary>
    /// Компонент запроса файлового типа
    /// </summary>
    public class FileComponent
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;

        public FileComponent() { }
        public FileComponent(FileComponentDto dto)
        {
            Name = dto.Name;
            Path = dto.Path;
            Key = dto.Key;
        }
    }
}
