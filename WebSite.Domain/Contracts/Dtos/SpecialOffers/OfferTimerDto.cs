using System.Text.Json.Serialization;

namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// DTO для таймера акции
    /// </summary>
    public class OfferTimerDto
    {
        /// <summary>
        /// Тип таймера
        /// </summary>
        [JsonPropertyName("type")]
        public TimerType Type { get; set; }

        /// <summary>
        /// Дата начала акции (для DateRange)
        /// </summary>
        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания акции
        /// </summary>
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Показывать ли секунды
        /// </summary>
        [JsonPropertyName("showSeconds")]
        public bool? ShowSeconds { get; set; }

        /// <summary>
        /// Показывать ли минуты
        /// </summary>
        [JsonPropertyName("showMinutes")]
        public bool? ShowMinutes { get; set; }

        /// <summary>
        /// Показывать ли часы
        /// </summary>
        [JsonPropertyName("showHours")]
        public bool? ShowHours { get; set; }

        /// <summary>
        /// Показывать ли дни
        /// </summary>
        [JsonPropertyName("showDays")]
        public bool? ShowDays { get; set; }
    }
}
