using Youme.Services;
using Youme.Views;

namespace Youme.ViewModels
{
    public class SettingsVM:ViewModel
    {
        private Settings? view;
        public SettingsVM(Settings view)
        {
            this.view = view;
        }

        public string InputProjectPrompt
        {
            get { return StorageService.InputProjectPrompt; }
            set 
            {
                StorageService.InputProjectPrompt = value;
                OnPropertyChanged();
            }
        }

        public string StructurePromptLocal
        {
            get { return StorageService.StructurePromptLocal; }
            set 
            {
                StorageService.StructurePromptLocal = value;
                OnPropertyChanged();
            }
        }

        public string StructurePromptGlobal
        {
            get { return StorageService.StructurePromptGlobal; }
            set 
            { 
                StorageService.StructurePromptGlobal = value;
                OnPropertyChanged();
            }
        }

        public string UserSettingsPrompt
        {
            get { return StorageService.UserSettingsPrompt; }
            set 
            { 
                StorageService.UserSettingsPrompt = value;
                OnPropertyChanged();
            }
        }

        public string StyleFileBlock
        {
            get { return StorageService.StyleFileBlock; }
            set 
            { 
                StorageService.StyleFileBlock = value;
                OnPropertyChanged();
            }
        }
    }
}
