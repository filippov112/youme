using System.Collections.ObjectModel;

namespace Youme.ViewModels
{
    public class SearchVM<T> : ViewModel
        where T : class
    {
        private ObservableCollection<SearchElement> _allItems = [];
        private ObservableCollection<SearchElement> _filteredItems = [];
        private string _searchText = string.Empty;
        private bool _isPopupOpen = false;
        private SearchElement? _selectedItem = null;


        public SearchVM()
        {
            CheckText = (item) => item?.ToString() ?? string.Empty;
            DisplayText = (item) => item?.ToString() ?? string.Empty;
        }
        public SearchVM(Func<T, string> checkText, Func<T, string> displayText, Action<T>? action = null)
        {
            CheckText = checkText;
            DisplayText = displayText;
            if (action != null)
                ItemSelected += action;
        }

        /// <summary>
        /// Общий список поиска
        /// </summary>
        public IEnumerable<T> Items
        {
            get => _allItems.Select(x => x.Element);
            set => _allItems = new ObservableCollection<SearchElement>(value.Select(x => new SearchElement() { Element = x, ResultText = DisplayText(x) }));
        }

        /// <summary>
        /// Отображаемый список результатов поиска
        /// </summary>
        public ObservableCollection<SearchElement> FilteredItems
        {
            get => _filteredItems;
            set
            {
                _filteredItems = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текст в поисковой строке
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterItems();
            }
        }

        /// <summary>
        /// Открытие/закрытие выпадающего списка результатов
        /// </summary>
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                _isPopupOpen = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранный элемент результирующего списка
        /// </summary>
        public SearchElement? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {
                    ItemSelected?.Invoke(value.Element);
                    //SearchText = GetDisplayText(value);
                    //IsPopupOpen = false;
                }
            }
        }
        /// <summary>
        /// Внешний выбор элемента из кода
        /// </summary>
        /// <param name="item"></param>
        public void SelectItem(T item)
        {
            var sItem = _allItems.FirstOrDefault(x => x.Element == item);
            if (sItem != null)
            {
                SelectedItem = sItem;
            }
        }

        /// <summary>
        /// Поиск
        /// </summary>
        protected virtual void FilterItems()
        {
            IsPopupOpen = !string.IsNullOrWhiteSpace(SearchText);
            if (IsPopupOpen)
            {
                FilteredItems = new ObservableCollection<SearchElement>(
                    _allItems
                        .Where(item => CheckText(item.Element).Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .Select(item => new SearchElement() { Element = item.Element, ResultText = DisplayText(item.Element) })
                );
            }
            SelectedItem = null;
        }


        /// <summary>
        /// Текст для сравнения с поисковой строкой
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Func<T, string> CheckText;


        /// <summary>
        /// Отображаемый текст результатов выборки
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Func<T, string> DisplayText;



        /// <summary>
        /// Выбор элемента из списка найденных
        /// </summary>
        /// <param name="item"></param>
        public event Action<T>? ItemSelected;

        /// <summary>
        /// Элемент выборки
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        public class SearchElement
        {
            public required T Element { get; set; }
            public string ResultText { get; set; } = string.Empty;
        }
    }
}
