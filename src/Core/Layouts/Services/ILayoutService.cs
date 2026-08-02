using Core.Layouts.DTO;

namespace Core.Layouts.Services
{
    public interface ILayoutService
    {
        /// <summary>
        /// Структура активной компоновки для сборки запроса
        /// </summary>
        string ActiveLayoutStructure { get; }
        Guid ActiveLayoutID { get; }

        /// <summary>
        /// Выбрать активную компоновку
        /// </summary>
        /// <param name="id"></param>
        Task ChangeActiveLayout(Guid id);

        /// <summary>
        /// Получить компоновки
        /// </summary>
        /// <param name="commonLayouts"></param>
        /// <param name="projectLayouts"></param>
        /// <returns></returns>
        Task<List<LayoutDto>> GetLayouts(bool commonLayouts, bool projectLayouts);

        /// <summary>
        /// Сохранить компоновки
        /// </summary>
        /// <param name="layouts"></param>
        /// <returns></returns>
        Task SaveAllLayouts(List<LayoutDto> layouts);
    }
}
