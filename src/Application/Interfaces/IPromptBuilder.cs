namespace Application.Interfaces
{
    public interface IPromptBuilder
    {
        public Task<string> GetPrompt(List<string> filePath, string queryText);
    }
}
