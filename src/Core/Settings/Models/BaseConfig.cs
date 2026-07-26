using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Settings.Models
{
    /// <summary>
    /// Базовая модель конфигурации.
    /// Содержит переопределяемые параметры
    /// </summary>
    public abstract class BaseConfig
    {
        // Условные обозначения ключей для замены в структурах промпта.
        public string IntroductionKey { get; set; } = "##intro##"; // Блок введения
        public string ContextKey { get; set; } = "##context##"; // Контекстный блок
        public string RulesKey { get; set; } = "##rules##"; // Блок правил
        public string QueryKey { get; set; } = "##query##"; // Блок запроса
        public string FilePathKey { get; set; } = "##path##"; // Расположение файла
        public string FileContentKey { get; set; } = "##content##"; // Содержимое файла

        // Структуры
        public string PromptStructure { get; set; } = $"##intro##\n`````\n##context##\n`````\n##query##\n\n##rules##"; // Структура промпта
        public string FileStructure = $"File: ##path##\n````\n##content##\n````"; // Структура отдельного файла в блоке контекста

        // Значения по умолчанию
        public string IntroductionText { get; set; } = ""; // Текст блока введения
        public string RulesText { get; set; } = ""; // Текст блока правил

        
    }
}
