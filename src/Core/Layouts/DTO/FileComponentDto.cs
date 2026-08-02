using Core.Layouts.Model;

namespace Core.Layouts.DTO
{
    public class FileComponentDto : FileComponent
    {
        public FileComponentDto() { }
        public FileComponentDto(FileComponent dto)
        {
            Name = dto.Name;
            Path = dto.Path;
            Key = dto.Key;
        }
    }
}
