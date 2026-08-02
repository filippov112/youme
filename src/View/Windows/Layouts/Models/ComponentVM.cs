using Core.Layouts.DTO;
using View.Other;

namespace View.Windows.Layouts.Models
{
    public class ComponentVM : ViewModel
    {
        public string Name { get; set { field = value; OnPropertyChanged(); } }
        public string Key { get; set { field = value; OnPropertyChanged(); } }

        public ComponentVM(FileComponentDto dto)
        {
            Name = dto.Name;
            Key = dto.Key;
        }
    }
}
