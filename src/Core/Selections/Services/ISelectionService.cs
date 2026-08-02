using Core.Selections.Models;

namespace Core.Selections.Services
{
    /// <summary>
    /// Сервис управления выборками файлов
    /// </summary>
    public interface ISelectionService
    {
        /// <summary>
        /// Получить список выборок
        /// </summary>
        /// <returns></returns>
        public Task<List<Selection>> GetSelections();

        /// <summary>
        /// Сохранить выборки
        /// </summary>
        /// <param name="selections">Список выборок</param>
        /// <returns></returns>
        public Task SaveSelections(IEnumerable<Selection> selections);
    }
}
