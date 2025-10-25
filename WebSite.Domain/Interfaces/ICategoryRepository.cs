using WebSite.Domain.Models;

namespace WebSite.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default); 
    }
}
