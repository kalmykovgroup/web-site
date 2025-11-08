namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// Типы специальных предложений
    /// </summary>
    public enum OfferType
    {
        /// <summary>
        /// Процентная скидка
        /// </summary>
        PercentageDiscount,

        /// <summary>
        /// Фиксированная скидка в рублях
        /// </summary>
        FixedDiscount,

        /// <summary>
        /// Акция "2+1" или "3+2"
        /// </summary>
        BuyGet,

        /// <summary>
        /// Ограниченное предложение
        /// </summary>
        LimitedOffer,

        /// <summary>
        /// Сезонная распродажа
        /// </summary>
        SeasonalSale,

        /// <summary>
        /// Новинка
        /// </summary>
        NewArrival
    }
}
