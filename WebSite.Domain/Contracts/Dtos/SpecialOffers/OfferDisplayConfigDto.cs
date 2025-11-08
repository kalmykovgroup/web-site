using System.Text.Json.Serialization;

namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// Конфигурация отображения акции
    /// </summary>
    public class OfferDisplayConfigDto
    {
        /// <summary>
        /// Цвет фона карточки (например: "#FF6B6B")
        /// </summary>
        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; } = string.Empty;

        /// <summary>
        /// Цвет текста (например: "#FFFFFF")
        /// </summary>
        [JsonPropertyName("textColor")]
        public string TextColor { get; set; } = string.Empty;

        /// <summary>
        /// Цвет акцента для кнопок и бейджей (например: "#FF8E53")
        /// </summary>
        [JsonPropertyName("accentColor")]
        public string AccentColor { get; set; } = string.Empty;

        /// <summary>
        /// Размер карточки: small, medium, large
        /// </summary>
        [JsonPropertyName("size")]
        public string Size { get; set; } = string.Empty;

        /// <summary>
        /// Показывать ли градиент
        /// </summary>
        [JsonPropertyName("showGradient")]
        public bool ShowGradient { get; set; }

        /// <summary>
        /// Позиция таймера: top, bottom, overlay
        /// </summary>
        [JsonPropertyName("timerPosition")]
        public string TimerPosition { get; set; } = string.Empty;

        /// <summary>
        /// Тип раскладки
        /// </summary>
        [JsonPropertyName("layout")]
        public LayoutType Layout { get; set; }

        /// <summary>
        /// Настройки для overlay раскладки
        /// </summary>
        [JsonPropertyName("overlayConfig")]
        public OverlayConfigDto? OverlayConfig { get; set; }

        /// <summary>
        /// Настройки для раскладки с изображением и контентом
        /// </summary>
        [JsonPropertyName("imageWithContentConfig")]
        public ImageWithContentConfigDto? ImageWithContentConfig { get; set; }
    }
}
