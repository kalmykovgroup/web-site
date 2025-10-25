namespace WebSite.Domain.Models
{
    public class CategoryImage
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;

        public int Order { get; set; }

        // Foreign Key
        public Guid CategoryId { get; set; }  
        public Category Category { get; set; } = null!;
    }
}
