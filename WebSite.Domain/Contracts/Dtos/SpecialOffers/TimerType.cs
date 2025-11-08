namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// Тип таймера для акции
    /// </summary>
    public enum TimerType
    {
        /// <summary>
        /// Обратный отсчет до определенной даты
        /// </summary>
        Countdown,

        /// <summary>
        /// Период действия (от-до)
        /// </summary>
        DateRange,

        /// <summary>
        /// Без таймера
        /// </summary>
        None
    }
}
