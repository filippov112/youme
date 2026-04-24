using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public interface IContentBuilder
    {
        /// <summary>
        /// Функция парсинга файла
        /// </summary>
        /// <param name="fullpath">Путь к файлу</param>
        /// <returns>Содержимое файла</returns>
        public string ParseFile(string fullpath);

        /// <summary>
        /// Можно ли парсить конкретный файл
        /// </summary>
        /// <param name="fullpath">Абсолютный путь к файлу</param>
        public bool ShouldInclude(string fullpath);

        /// <summary>
        /// Формирует контекст проекта
        /// </summary>
        public string Build(List<string> files);
    }
}
