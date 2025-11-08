using System.Text.Json.Serialization;

namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// DTO для специального предложения
    /// </summary>
    public class SpecialOfferDto
    {
        /// <summary>
        /// Уникальный идентификатор акции
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Тип акции
        /// </summary>
        [JsonPropertyName("type")]
        public OfferType Type { get; set; }

        /// <summary>
        /// Название акции
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание акции
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// URL изображения для акции
        /// </summary>
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Alt текст для изображения
        /// </summary>
        [JsonPropertyName("imageAlt")]
        public string? ImageAlt { get; set; }

        /// <summary>
        /// Конфигурация отображения
        /// </summary>
        [JsonPropertyName("displayConfig")]
        public OfferDisplayConfigDto DisplayConfig { get; set; } = new();

        /// <summary>
        /// Таймер акции (опционально)
        /// </summary>
        [JsonPropertyName("timer")]
        public OfferTimerDto? Timer { get; set; }

        /// <summary>
        /// Порядок отображения
        /// </summary>
        [JsonPropertyName("order")]
        public int Order { get; set; }

        /// <summary>
        /// Активна ли акция
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Дополнительные данные в зависимости от типа акции
        /// </summary>
        [JsonPropertyName("metadata")]
        public OfferMetadataDto? Metadata { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
