namespace WebSite.Domain.Models
{ 
    public class Category
    {
        public Guid Id { get; set; }
        public string IconUrl { get; set; } = string.Empty;
        public string IconAlt { get; set; } = string.Empty;
        public string BackgroundUrl { get; set; } = string.Empty;
        public string BackgroundAlt { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
        public List<CategoryImage> Images { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
