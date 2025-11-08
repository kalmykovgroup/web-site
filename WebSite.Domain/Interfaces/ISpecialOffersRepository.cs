using WebSite.Domain.Models;

namespace WebSite.Domain.Interfaces
{
    public interface ISpecialOffersRepository
    {
        Task<List<SpecialOffer>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<SpecialOffer>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<SpecialOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
