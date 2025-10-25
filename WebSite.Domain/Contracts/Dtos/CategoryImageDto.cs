using WebSite.Domain.Models;

namespace WebSite.Domain.Contracts.Dtos
{
    public class CategoryImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;

        public int Order { get; set; }

        // Foreign Key
        public Guid CategoryId { get; set; }  
    }
}
