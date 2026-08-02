using Core.Layouts.DTO;
using Core.Layouts.Services;
using Core.Settings.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using View.Other;
using View.Services;
using View.Windows.Layouts.Models;

namespace View.Windows.Layouts
{
    public class LayoutSettingsWindowVM : ViewModel
    {
        public LayoutSettingsWindowVM(ILayoutService layoutService, IDialogService dialogService, IFileComponentService componentService, IConfigService configService)
        {
            _layoutService = layoutService;
            _dialogService = dialogService;
            _componentService = componentService;

            CreateCommonLayoutCommand = new RelayCommand(_ => CreateLayout(true));
            CreateProjectLayoutCommand = new RelayCommand(_ => CreateLayout(false));
            UpdateLayoutCommand = new RelayCommand(UpdateLayout, () => SelectedLayout is not null);
            DeleteLayoutCommand = new RelayCommand(DeleteLayout, () => SelectedLayout is not null);

            SaveCommand = new RelayCommand(Save, CanSave);

            LoadComponents();
            LoadLayouts();

            Task.Run(async () =>
            {
                try
                {
                    var config = await configService.GetConfigAsync();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ContextKey = config.Local is null ? config.Global.ContextKey : config.Local.ContextKey;
                        QueryKey = config.Local is null ? config.Global.QueryKey : config.Local.QueryKey;
                    });
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _dialogService.ShowError($"Ошибка загрузки компоновок: {ex.Message}");
                    });
                }
            });
        }
        private IFileComponentService _componentService;
        private ILayoutService _layoutService;
        private IDialogService _dialogService;


        #region Layouts
        private void LoadLayouts()
        {
            Task.Run(async () =>
            {
                try
                {
                    var layouts = await _layoutService.GetLayouts(true, true);

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var l in layouts)
                        {
                            if (l.IsCommon)
                                CommonLayouts.Add(new(l));
                            else
                                ProjectLayouts.Add(new(l));
                        }
                    });
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _dialogService.ShowError($"Ошибка загрузки компоновок: {ex.Message}");
                    });
                }
            });
        }

        /// <summary>
        /// Компоновки
        /// </summary>
        public ObservableCollection<LayoutVM> CommonLayouts { get; set { field = value; OnPropertyChanged(); } } = [];
        public ObservableCollection<LayoutVM> ProjectLayouts { get; set { field = value; OnPropertyChanged(); } } = [];
        public int ActiveLayoutTabIndex { get; set { field = value; OnPropertyChanged(); } } = 0;
        public LayoutVM? SelectedLayout
        {
            get; set
            {
                field = value;
                OnPropertyChanged();
                LoadLayout(value);
            }
        }
        private void LoadLayout(LayoutVM? layout)
        {
            CurrentLayoutStructure = layout?.Structure ?? string.Empty;
            CurrentLayoutName = layout?.Name ?? string.Empty;
            _currentLayoutID = layout?.ID ?? Guid.Empty;
        }

        public string CurrentLayoutName { get; set { field = value; OnPropertyChanged(); } } = string.Empty;
        private Guid _currentLayoutID = Guid.Empty;
        public ICommand CreateCommonLayoutCommand { get; set; }
        public ICommand CreateProjectLayoutCommand { get; set; }
        public ICommand DeleteLayoutCommand { get; set; }
        public ICommand UpdateLayoutCommand { get; set; }

        private void CreateLayout(bool isCommon)
        {
            LayoutDto dto = new() { Name = CurrentLayoutName, ID = Guid.NewGuid(), Structure = CurrentLayoutStructure, IsCommon = isCommon };
            var layout = new LayoutVM(dto);
            if (isCommon)
                CommonLayouts.Add(layout);
            else
                ProjectLayouts.Add(layout);
            layoutsChanged = true;
            SelectedLayout = layout;
            ActiveLayoutTabIndex = isCommon ? 0 : 1;
        }

        private void UpdateLayout(object? sender)
        {
            SelectedLayout!.Name = CurrentLayoutName;
            SelectedLayout!.Structure = CurrentLayoutStructure;
            layoutsChanged = true;
        }
        private void DeleteLayout(object? sender)
        {
            if (SelectedLayout!.DTO.IsCommon)
                CommonLayouts.Remove(SelectedLayout!);
            else
                ProjectLayouts.Remove(SelectedLayout!);
            layoutsChanged = true;
            SelectedLayout = null;
        }
        #endregion

        #region Editor
        /// <summary>
        /// Структура выбранной компановки
        /// </summary>
        public string CurrentLayoutStructure { get; set { field = value; OnPropertyChanged(); } } = string.Empty;
        #endregion

        #region Components
        public string ContextKey { get; set { field = value; OnPropertyChanged(); } }
        public string QueryKey { get; set { field = value; OnPropertyChanged(); } }
        public ObservableCollection<ComponentVM> Components { get; set { field = value; OnPropertyChanged(); } } = [];
        private void LoadComponents()
        {
            try
            {
                var components = Task.Run(_componentService.GetAllComponents).Result;

                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var c in components)
                        Components.Add(new(c));
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка загрузки файловых компонентов: {ex.Message}");
            }
        }
        #endregion



        #region Saving
        private bool _sCh = false;
        private bool layoutsChanged
        {
            get => _sCh;
            set
            {
                _sCh = value;
            }
        }
        public ICommand SaveCommand { get; private set; }
        public void Save(object? sender)
        {
            Task.Run(async () =>
            {
                await _layoutService.SaveAllLayouts([..CommonLayouts.Select(x => x.DTO), ..ProjectLayouts.Select(y => y.DTO)]);
            });
            layoutsChanged = false;
        }
        public bool CanSave() => layoutsChanged;
        #endregion

        #region Closing
        public bool ApproveClosing()
        {
            if (CanSave())
            {
                var result = _dialogService.ShowYesNoDialog("У вас есть несохраненные изменения. Вы уверены, что хотите закрыть окно?", "Подтверждение закрытия");
                if (!result)
                    return false;
            }
            return true;
        }
        #endregion
    }
}
