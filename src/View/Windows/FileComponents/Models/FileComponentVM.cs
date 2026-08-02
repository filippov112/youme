using Core.Layouts.DTO;
using View.Other;

namespace View.Windows.FileComponents.Models
{
    public class FileComponentVM : ViewModel
    {
        public string Name { get; set { field = value; OnPropertyChanged(); } }
        public string Path { get; set { field = value; OnPropertyChanged(); } }
        public string Key { get; set { field = value; OnPropertyChanged(); } }

        public FileComponentVM(FileComponentDto dto)
        {
            Name = dto.Name;
            Path = dto.Path;
            Key = dto.Key;
        }

        public FileComponentDto DTO => new() { Name = Name, Path = Path, Key = Key };
    }
}
