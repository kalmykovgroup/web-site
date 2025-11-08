namespace WebSite.Domain.Models
{
    /// <summary>
    /// Entity модель для специального предложения
    /// </summary>
    public class SpecialOffer
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // Храним как строку
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
        public string DisplayConfig { get; set; } = string.Empty; // JSON
        public string? Timer { get; set; } // JSON
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Metadata { get; set; } // JSON
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
