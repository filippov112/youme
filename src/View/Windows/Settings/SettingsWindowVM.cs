using Core.Settings.Models.DTO;
using Core.Settings.Services;
using System.Windows;
using System.Windows.Input;
using View.Other;
using View.Services;

namespace View.Windows.Settings
{
    public class SettingsWindowVM : ViewModel
    {
        private readonly IConfigService _configService;
        private ConfigDto? _allConfig;
        private ConfigDto? _originalConfig;
        private bool _isGlobalSelected = true;
        private bool _isLocalSelected;
        private bool _isLocalEnabled;

        private readonly IDialogService _dialogs;

        public SettingsWindowVM(IConfigService configService, IDialogService dialogs)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _dialogs = dialogs;
            SelectTabCommand = new RelayCommand<string>(SelectTab);
            SaveCommand = new RelayCommand(Save, CanSave);
            ResetToDefaultCommand = new RelayCommand(ResetToDefault);
            CancelCommand = new RelayCommand(Cancel);

            try
            {
                var config = Task.Run(_configService.GetConfigAsync).Result;

                App.Current.Dispatcher.Invoke(() =>
                {
                    _allConfig = config;
                    _originalConfig = _allConfig with
                    {
                        Global = _allConfig.Global.GetCopy(),
                        Local = _allConfig.Local?.GetCopy()
                    };
                    // Если нет локальной конфигурации, показываем глобальную
                    IsLocalEnabled = !string.IsNullOrEmpty(_configService.RootDirectory);
                    if (!IsLocalEnabled)
                    {
                        IsGlobalSelected = true;
                        IsLocalSelected = false;
                    }
                    OnPropertyChanged(nameof(GlobalConfig));
                    OnPropertyChanged(nameof(LocalConfig));
                    OnPropertyChanged(nameof(IsLocalEnabled));
                });
            }
            catch (Exception ex)
            {
                _dialogs.ShowError($"Ошибка загрузки настроек: {ex.Message}");
            }
        }


        public GlobalConfigDto? GlobalConfig => _allConfig?.Global;
        public LocalConfigDto? LocalConfig => _allConfig?.Local;

        public bool IsGlobalSelected
        {
            get => _isGlobalSelected;
            set
            {
                if (_isGlobalSelected != value)
                {
                    _isGlobalSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentTabTitle));
                }
            }
        }

        public bool IsLocalSelected
        {
            get => _isLocalSelected;
            set
            {
                if (_isLocalSelected != value)
                {
                    _isLocalSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentTabTitle));
                }
            }
        }

        public bool IsLocalEnabled
        {
            get => _isLocalEnabled;
            set
            {
                if (_isLocalEnabled != value)
                {
                    _isLocalEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentTabTitle => IsGlobalSelected ? "Глобальная конфигурация" : "Проектная конфигурация";

        public ICommand SelectTabCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ResetToDefaultCommand { get; }
        public ICommand CancelCommand { get; }

        private void SelectTab(string? tab)
        {
            if (tab == "Global")
            {
                IsGlobalSelected = true;
                IsLocalSelected = false;
            }
            else if (tab == "Local" && IsLocalEnabled)
            {
                IsGlobalSelected = false;
                IsLocalSelected = true;
            }
        }

        private bool CanSave() => _allConfig != null;

        private async void Save(object? p)
        {
            try
            {
                if (_allConfig is null)
                    return;
                await _configService.SaveConfigAsync(_allConfig);
                _originalConfig = _allConfig with
                {
                    Global = _allConfig.Global.GetCopy(),
                    Local = _allConfig.Local?.GetCopy()
                };
            }
            catch (Exception ex)
            {
                _dialogs.ShowError($"Ошибка сохранения настроек: {ex.Message}");
            }
        }

        private async void ResetToDefault(object? p)
        {
            var result = _dialogs.ShowYesNoDialog("Вы уверены, что хотите сбросить все настройки к заводским значениям?", "Подтверждение сброса");

            if (result)
            {
                try
                {
                    var defaultConfig = _configService.GetDefaultConfig();
                    _allConfig = defaultConfig;

                    IsLocalEnabled = _configService.ProjectOpened;

                    OnPropertyChanged(nameof(GlobalConfig));
                    OnPropertyChanged(nameof(LocalConfig));
                    OnPropertyChanged(nameof(IsLocalEnabled));

                    if (!IsLocalEnabled && IsLocalSelected)
                    {
                        IsGlobalSelected = true;
                        IsLocalSelected = false;
                    }
                }
                catch (Exception ex)
                {
                    _dialogs.ShowError($"Ошибка сброса настроек: {ex.Message}");
                }
            }
        }

        private void Cancel(object? _)
        {
            if (OnCloced())
                CloseWindow();
        }

        public bool OnCloced()
        {
            if (HasChanges())
            {
                var result = _dialogs.ShowYesNoDialog("У вас есть несохраненные изменения. Вы уверены, что хотите закрыть окно?",
                                           "Подтверждение закрытия");

                if (!result)
                    return false;
            }
            return true;
        }

        private bool HasChanges()
        {
            if (_originalConfig == null || _allConfig == null)
                return false;

            // Сравнение глобальной конфигурации
            if (!CompareGlobalConfigs(_originalConfig.Global, _allConfig.Global))
                return true;

            // Сравнение локальной конфигурации
            if (_originalConfig.Local != null && _allConfig.Local != null)
            {
                if (!CompareLocalConfigs(_originalConfig.Local, _allConfig.Local))
                    return true;
            }
            else if (_originalConfig.Local != null || _allConfig.Local != null)
            {
                return true;
            }

            return false;
        }

        private static bool CompareLocalConfigs(LocalConfigDto a, LocalConfigDto b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;

            return a.ContextKey == b.ContextKey &&
                   a.QueryKey == b.QueryKey &&
                   a.FilePathKey == b.FilePathKey &&
                   a.FileContentKey == b.FileContentKey &&
                   a.FileStructure == b.FileStructure;
        }

        private static bool CompareGlobalConfigs(GlobalConfigDto a, GlobalConfigDto b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;

            return a.ContextKey == b.ContextKey &&
                   a.QueryKey == b.QueryKey &&
                   a.FilePathKey == b.FilePathKey &&
                   a.FileContentKey == b.FileContentKey &&
                   a.FileStructure == b.FileStructure &&

                   a.DocsTemplate == b.DocsTemplate;

        }

        private void CloseWindow()
        {
            if (System.Windows.Application.Current.Windows.Count > 0)
            {
                foreach (var window in System.Windows.Application.Current.Windows)
                {
                    if (window is Window w && w.DataContext == this)
                    {
                        w.Close();
                        break;
                    }
                }
            }
        }

    }

}