using Core.Layouts.Model;

namespace Core.Layouts.DTO
{
    public class LayoutDto : Layout
    {
        public bool IsCommon { get; set; }

        public LayoutDto() { }
        public LayoutDto(Layout model, bool isCommon)
        {
            IsCommon = isCommon;
            Name = model.Name;
            Structure = model.Structure;
            ID = model.ID;
        }
    }
}
