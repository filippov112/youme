using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using TreeView = System.Windows.Controls.TreeView;
using UserControl = System.Windows.Controls.UserControl;

namespace View.Windows.Main.Explorer
{
    /// <summary>
    /// Логика взаимодействия для Explorer.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        private Point _startPoint; // Точка начала перемещения
        private ExplorerItemVM? _draggedItem; // Элемент, который перетаскивается
        private TreeViewItem? _visualDropTarget; // Для визуального выделения
        private ExplorerVM VM => (ExplorerVM)DataContext;


        public ExplorerControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Транслирует выбор элемента в TreeView в открытие документа в редакторе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ExplorerItemVM item)
            {
                if (item == null || item.Type != ItemType.File)
                    return;
                VM.OpenFile?.Invoke(item.FullPath);
            }
        }


        private void SelectItemUnderMouse(object sender, MouseButtonEventArgs e)
        {
            var treeView = (TreeView)sender;
            var element = e.OriginalSource as DependencyObject;
            while (element != null && element != treeView)
            {
                if (element is TreeViewItem item)
                {
                    item.Focus();
                    if (!item.IsSelected)
                    {
                        item.IsSelected = true;
                    }
                    break;
                }
                element = VisualTreeHelper.GetParent(element);
            }
        }


        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectItemUnderMouse(sender, e);
        }

        #region Перенос элементов дерева (Drag & Drop)

        /// <summary>
        /// Захват элемента дерева для перетаскивания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            TreeViewItem? treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (treeViewItem != null)
            {
                _draggedItem = treeViewItem.DataContext as ExplorerItemVM;
            }
        }

        /// <summary>
        /// Обработка перемещения мыши для инициации Drag & Drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedItem != null)
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = _startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (_draggedItem != null)
                    {
                        var data = ((ExplorerVM)DataContext).MoveToQuery(_draggedItem);
                        // Инициируем операцию Drag & Drop
                        DragDrop.DoDragDrop(treeExplorer, data, DragDropEffects.Move);
                    }
                }
            }
        }

        /// <summary>
        /// Обработка наведения мыши на элемент во время Drag & Drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ExplorerItemVM"))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            // Сброс предыдущего визуального выделения
            if (_visualDropTarget != null)
            {
                if (_visualDropTarget.Tag?.ToString() == "DropTargetHighlight")
                {
                    _visualDropTarget.ClearValue(TreeViewItem.BackgroundProperty);
                    _visualDropTarget.Tag = null;
                }
                _visualDropTarget = null;
            }

            Point mousePos = e.GetPosition(treeExplorer);
            DependencyObject element = (DependencyObject)treeExplorer.InputHitTest(mousePos);

            TreeViewItem? targetItem = FindAncestor<TreeViewItem>(element);
            ExplorerItemVM? targetVM = targetItem?.DataContext as ExplorerItemVM;

            // Если не нашли под курсором, возможно, над пустой областью папки
            if (targetVM == null && targetItem == null)
            {
                DependencyObject current = element;
                while (current != null && current != treeExplorer)
                {
                    if (current is TreeViewItem currentTVI)
                    {
                        targetItem = currentTVI;
                        targetVM = targetItem.DataContext as ExplorerItemVM;
                        break;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }
            }

            if (targetVM != null)
            {
                if (e.Data.GetData("ExplorerItemVM") is ExplorerItemVM draggedItem)
                {
                    // Определяем целевую папку для сброса
                    ExplorerItemVM? destinationFolder = null;
                    if (targetVM.Type == ItemType.Folder)
                    {
                        // Сброс на папку
                        destinationFolder = targetVM;
                    }
                    else if (targetVM.Type == ItemType.File)
                    {
                        // Сброс на файл - используем родительскую папку файла
                        destinationFolder = targetVM.Parent;
                    }

                    if (destinationFolder != null)
                    {
                        if (destinationFolder != draggedItem && !IsDescendantOf(destinationFolder, draggedItem))
                        {
                            e.Effects = DragDropEffects.Move;
                            if (targetItem != null)
                            {
                                // Если сброс на файл, выделяем родительский элемент
                                TreeViewItem? visualItemToHighlight = targetItem;
                                if (targetVM.Type == ItemType.File && targetVM.Parent != null)
                                {
                                    visualItemToHighlight = GetTreeViewItemForDataContext(treeExplorer, destinationFolder);
                                }

                                if (visualItemToHighlight != null)
                                {
                                    visualItemToHighlight.Background = System.Windows.Media.Brushes.LightBlue;
                                    visualItemToHighlight.Tag = "DropTargetHighlight";
                                    _visualDropTarget = visualItemToHighlight;
                                }
                                else
                                {
                                    targetItem.Background = System.Windows.Media.Brushes.LightBlue;
                                    targetItem.Tag = "DropTargetHighlight";
                                    _visualDropTarget = targetItem;
                                }
                            }
                        }
                        else
                        {
                            e.Effects = DragDropEffects.None;
                        }
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Обработка сброса элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            // Сбрасываем визуальное выделение
            if (_visualDropTarget != null)
            {
                _visualDropTarget.ClearValue(TreeViewItem.BackgroundProperty);
                _visualDropTarget.Tag = null;
                _visualDropTarget = null;
            }

            if (!e.Data.GetDataPresent("ExplorerItemVM"))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            if (e.Data.GetData("ExplorerItemVM") is not ExplorerItemVM draggedItem)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            // Найдем целевой элемент снова на момент Drop
            Point mousePos = e.GetPosition(treeExplorer);
            DependencyObject element = (DependencyObject)treeExplorer.InputHitTest(mousePos);
            TreeViewItem? targetItem = FindAncestor<TreeViewItem>(element);
            ExplorerItemVM? targetVM = targetItem?.DataContext as ExplorerItemVM;

            if (targetVM == null && targetItem == null)
            {
                DependencyObject current = element;
                while (current != null && current != treeExplorer)
                {
                    if (current is TreeViewItem currentTVI)
                    {
                        targetItem = currentTVI;
                        targetVM = targetItem.DataContext as ExplorerItemVM;
                        break;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }
            }

            if (targetVM != null)
            {
                // Определяем целевую папку для сброса
                ExplorerItemVM? destinationFolder = null;
                if (targetVM.Type == ItemType.Folder)
                {
                    destinationFolder = targetVM;
                }
                else if (targetVM.Type == ItemType.File)
                {
                    destinationFolder = targetVM.Parent;
                }

                if (destinationFolder != null)
                {
                    // Получаем ViewModel для доступа к методу MoveItem
                    if (DataContext is ExplorerVM explorerVM)
                    {
                        explorerVM.MoveObjectBetweenDirectories(draggedItem.FullPath, Path.Combine(destinationFolder.FullPath, draggedItem.Name));
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                else
                {
                    // destinationFolder не определена
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                // Цель не найдена
                e.Effects = DragDropEffects.None;
            }

            _draggedItem = null; // Сброс состояния
            e.Handled = true;
        }

        // Вспомогательный метод для проверки, является ли 'potentialParent' родителем 'child'
        // Имя изменено на IsDescendantOf для соответствия оригинальному комментарию
        private static bool IsDescendantOf(ExplorerItemVM child, ExplorerItemVM potentialParent)
        {
            var current = child.Parent;
            while (current != null)
            {
                if (current == potentialParent)
                    return true;
                current = current.Parent;
            }
            return false;
        }

        /// <summary>
        /// Поиск нужного типа элемента в визуальном дереве
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
        {
            do
            {
                if (current is T t)
                    return t;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        /// <summary>
        /// Находит TreeViewItem, соответствующий заданному DataContext (ExplorerElementVM).
        /// Это вспомогательный метод для визуального выделения.
        /// </summary>
        /// <param name="treeView">Контрол TreeView</param>
        /// <param name="dataContext">Объект данных для поиска</param>
        /// <returns>TreeViewItem или null, если не найден</returns>
        private static TreeViewItem? GetTreeViewItemForDataContext(ItemsControl container, object dataContext)
        {
            if (container == null) return null;

            if (container.DataContext == dataContext)
            {
                return container as TreeViewItem;
            }

            // Проходим по контейнерам элементов
            for (int i = 0; i < container.Items.Count; i++)
            {
                if (container.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem item)
                {
                    if (item.DataContext == dataContext)
                    {
                        return item;
                    }

                    // Рекурсивно проверяем дочерние элементы
                    var subContainer = item;
                    var result = GetTreeViewItemForDataContext(subContainer, dataContext);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        #endregion
    }
}
