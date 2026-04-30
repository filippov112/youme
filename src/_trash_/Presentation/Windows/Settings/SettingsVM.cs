using Application.Models;
using Application.Services;
using Presentation.Other;

namespace Presentation.Windows.Settings
{
    public class SettingsVM(IStorageService storageService) : ViewModel
    {
        public GlobalConfig GlobalConfig { get; private set; } = storageService.GConfig.Copy();
        public LocalConfig LocalConfig { get; private set; } = storageService.LConfig.Copy();

        public string InputProjectPrompt
        {
            get => LocalConfig.InputProjectPrompt;
            set
            {
                if (LocalConfig.InputProjectPrompt != value)
                    BtnSaveIsActive = true;
                LocalConfig.InputProjectPrompt = value;
                OnPropertyChanged();
            }
        }

        public string StructurePromptLocal
        {
            get => LocalConfig.StructurePromptLocal;
            set
            {
                if (LocalConfig.StructurePromptLocal != value)
                    BtnSaveIsActive = true;
                LocalConfig.StructurePromptLocal = value;
                OnPropertyChanged();
            }
        }

        public string StructurePromptGlobal
        {
            get => GlobalConfig.StructurePromptGlobal;
            set
            {
                if (GlobalConfig.StructurePromptGlobal != value)
                    BtnSaveIsActive = true;
                GlobalConfig.StructurePromptGlobal = value;
                OnPropertyChanged();
            }
        }

        public string UserSettingsPrompt
        {
            get => GlobalConfig.UserSettingsPrompt;
            set
            {
                if (GlobalConfig.UserSettingsPrompt != value)
                    BtnSaveIsActive = true;
                GlobalConfig.UserSettingsPrompt = value;
                OnPropertyChanged();
            }
        }

        public string StyleFileBlock
        {
            get => GlobalConfig.StyleFileBlock;
            set
            {
                if (GlobalConfig.StyleFileBlock != value)
                    BtnSaveIsActive = true;
                GlobalConfig.StyleFileBlock = value;
                OnPropertyChanged();
            }
        }



        private bool _btnSaveIsActive = false;

        public bool BtnSaveIsActive
        {
            get => _btnSaveIsActive;
            set
            {
                _btnSaveIsActive = value;
                OnPropertyChanged();
            }
        }
    }
}
