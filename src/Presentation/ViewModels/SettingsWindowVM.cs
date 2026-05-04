using Application.Interfaces;
using Application.Models;
using Presentation.Other;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class SettingsWindowVM : ViewModel
    {
        private readonly IConfigService _cs;
        public SettingsWindowVM(IConfigService cs)
        {
            _cs = cs;
            Task.Run(async () => { Config = await _cs.GetAllConfigAsync(); });
            SaveCommand = new RelayCommand(async _ => { await _cs.SaveAllConfigAsync(Config); });
        }
        private AllConfigDto _config = new();
        public AllConfigDto Config
        {
            get => _config;
            set
            {
                _config = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand;
    }
}
