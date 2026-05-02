using Application.Models;

namespace Application.Interfaces
{
    public interface IConfigService
    {
        public string RootDirectory { get; }

        public Task SetRootDirectoryAsync(string rootDirectory);

        public Task<CombinationConfig> GetCombinationConfigAsync();

        public Task<AllConfigDto> GetAllConfigAsync();

        public Task SaveAllConfigAsync(AllConfigDto allConfig);

        public AllConfigDto GetAllConfigDefault();
    }
}
