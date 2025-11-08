namespace WebSite.Domain.Contracts.Dtos.SpecialOffers
{
    /// <summary>
    /// Тип раскладки карточки акции
    /// </summary>
    public enum LayoutType
    {
        /// <summary>
        /// Только изображение на весь экран
        /// </summary>
        ImageOnly,

        /// <summary>
        /// Изображение и контент рядом
        /// </summary>
        ImageWithContent,

        /// <summary>
        /// Описание поверх изображения
        /// </summary>
        Overlay
    }
}
