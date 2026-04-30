using Domain.Interfaces;


namespace Domain.Models.PromptComponents.FileComponents
{
    public class FileContent(string content) : IComponent
    {
        public string Key => "##content##";
        public string Value { get; set; } = content;
    }
}
