using Application.Models;

namespace Application.Interfaces
{
    public interface ISearchService
    {
        public Task<ProjectUnit?> FindMatchesAsync(string pattern);
    }
}
