using System.Text.Json.Serialization;

namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// Конфигурация overlay раскладки
    /// </summary>
    public class OverlayConfigDto
    {
        /// <summary>
        /// Позиция контента: top, center, bottom
        /// </summary>
        [JsonPropertyName("contentPosition")]
        public string ContentPosition { get; set; } = string.Empty;

        /// <summary>
        /// Выравнивание по горизонтали: left, center, right
        /// </summary>
        [JsonPropertyName("contentAlign")]
        public string ContentAlign { get; set; } = string.Empty;

        /// <summary>
        /// Затемнение фона (0-1)
        /// </summary>
        [JsonPropertyName("overlayOpacity")]
        public double OverlayOpacity { get; set; }

        /// <summary>
        /// Цвет затемнения в формате RGB без скобок (например: "0, 0, 0")
        /// </summary>
        [JsonPropertyName("overlayColor")]
        public string OverlayColor { get; set; } = string.Empty;
    }
}
