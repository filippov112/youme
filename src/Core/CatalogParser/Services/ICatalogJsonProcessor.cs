namespace Core.CatalogParser.Services
{
    public interface ICatalogJsonProcessor
    {
        /// <summary>
        /// Метод 1: Парсинг содержимого каталога и возврат в формате JSON
        /// </summary>
        /// <param name="includeFiles">Только указанные файлы</param>
        /// <returns>JSON-строка с иерархией каталога</returns>
        public Task<string> ParseDirectoryToJson(string[]? includeFiles = null);

        /// <summary>
        /// Метод 2: Объединение двух JSON-структур
        /// </summary>
        /// <param name="sourceJson">Исходный JSON (описания берутся из него)</param>
        /// <param name="structureJson">JSON со структурой для объединения</param>
        /// <returns>Объединенная JSON-строка</returns>
        public string MergeJsonStructures(string sourceJson, string structureJson);
    }
}
