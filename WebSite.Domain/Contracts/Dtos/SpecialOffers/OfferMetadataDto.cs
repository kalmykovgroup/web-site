using System.Text.Json.Serialization;

namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// Дополнительные данные акции в зависимости от типа
    /// </summary>
    public class OfferMetadataDto
    {
        /// <summary>
        /// Для PercentageDiscount: процент скидки
        /// </summary>
        [JsonPropertyName("discountPercent")]
        public int? DiscountPercent { get; set; }

        /// <summary>
        /// Для FixedDiscount: сумма скидки
        /// </summary>
        [JsonPropertyName("discountAmount")]
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// Для FixedDiscount: исходная цена
        /// </summary>
        [JsonPropertyName("originalPrice")]
        public decimal? OriginalPrice { get; set; }

        /// <summary>
        /// Для BuyGet: количество товаров для покупки
        /// </summary>
        [JsonPropertyName("buyQuantity")]
        public int? BuyQuantity { get; set; }

        /// <summary>
        /// Для BuyGet: количество товаров в подарок
        /// </summary>
        [JsonPropertyName("getQuantity")]
        public int? GetQuantity { get; set; }

        /// <summary>
        /// Для LimitedOffer: осталось товаров
        /// </summary>
        [JsonPropertyName("itemsLeft")]
        public int? ItemsLeft { get; set; }

        /// <summary>
        /// Для LimitedOffer: всего товаров
        /// </summary>
        [JsonPropertyName("totalItems")]
        public int? TotalItems { get; set; }

        /// <summary>
        /// Ссылка на категорию
        /// </summary>
        [JsonPropertyName("categoryId")]
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Ссылка на товар
        /// </summary>
        [JsonPropertyName("productId")]
        public Guid? ProductId { get; set; }
    }
}
