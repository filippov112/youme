using Core.Layouts.DTO;

namespace Core.Layouts.Model
{
    /// <summary>
    /// Компоновка сборки запроса
    /// </summary>
    public class Layout
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Structure { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;

        public Layout() { }
        public Layout(LayoutDto dto)
        {
            ID = dto.ID;
            Name = dto.Name;
            Structure = dto.Structure;
            IsActive = dto.IsActive;
        }
    }
}
