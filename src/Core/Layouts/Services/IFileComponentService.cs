using Core.Layouts.DTO;

namespace Core.Layouts.Services
{
    public interface IFileComponentService
    {
        /// <summary>
        /// Получить словарь замены для сборки промпта: ключ - текст файла
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetComponentsDict();

        /// <summary>
        /// Сохранить словарь файлов-компонентов
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        Task SaveAllComponents(List<FileComponentDto> files);

        /// <summary>
        /// Получить словарь файлов-компонентов
        /// </summary>
        /// <returns></returns>
        Task<List<FileComponentDto>> GetAllComponents();
    }
}
