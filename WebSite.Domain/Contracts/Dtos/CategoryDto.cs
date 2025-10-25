using WebSite.Domain.Models;

namespace WebSite.Domain.Contracts.Dtos
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string IconUrl { get; set; } = string.Empty;
        public string IconAlt { get; set; } = string.Empty;
        public string BackgroundUrl { get; set; } = string.Empty;
        public string BackgroundAlt { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int Order { get; set; } = 0;
        public string Color { get; set; } = string.Empty;
        public List<CategoryImageDto> Images { get; set; } = new();
    }
}
