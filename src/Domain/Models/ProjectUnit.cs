namespace Domain.Models
{
    public class ProjectUnit
    {
        // Иерархия
        public ProjectUnit? Parent { get; set; }
        public List<ProjectUnit> Children { get; set; } = [];

        // Свойства объекта
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool IsDirectory { get; set; } = false;
    }
}
