using System.Text.Json.Serialization;

namespace Core.CatalogParser.Models
{
    /// <summary>
    /// Представляет элемент каталога (файл или подкаталог)
    /// </summary>
    public class CatalogItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("children")]
        public List<CatalogItem>? Children { get; set; } = null;

        [JsonPropertyName("isDirectory")]
        public bool IsDirectory { get; set; }
    }
}
