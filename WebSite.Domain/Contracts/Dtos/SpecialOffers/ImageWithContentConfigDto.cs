using System.Text.Json.Serialization;

namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// Конфигурация раскладки с изображением и контентом рядом
    /// </summary>
    public class ImageWithContentConfigDto
    {
        /// <summary>
        /// Позиция изображения: left, right
        /// </summary>
        [JsonPropertyName("imagePosition")]
        public string ImagePosition { get; set; } = string.Empty;

        /// <summary>
        /// Ширина изображения в процентах (0-100)
        /// </summary>
        [JsonPropertyName("imageWidth")]
        public int ImageWidth { get; set; }
    }
}
