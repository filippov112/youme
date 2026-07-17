using System.Text.Json.Serialization;

namespace Core.CatalogParser.Models
{
    /// <summary>
    /// Представляет элемент каталога (файл или подкаталог)
    /// </summary>
    public class CatalogItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("children")]
        public List<CatalogItem> Children { get; set; }

        [JsonPropertyName("isDirectory")]
        public bool IsDirectory { get; set; }
    }
}
