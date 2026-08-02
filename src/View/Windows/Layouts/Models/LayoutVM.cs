using Core.Layouts.DTO;
using View.Other;

namespace View.Windows.Layouts.Models
{
    public class LayoutVM : ViewModel
    {
        
        public Guid ID { get => DTO.ID; set { DTO.ID = value; OnPropertyChanged(); } }
        public string Name { get => DTO.Name; set { DTO.Name = value; OnPropertyChanged(); } }
        public string Structure { get => DTO.Structure; set { DTO.Structure = value; OnPropertyChanged(); } }

        public LayoutVM(LayoutDto dto)
        {
            DTO = dto;
        }

        public LayoutDto DTO { get; set; }
    }
}
