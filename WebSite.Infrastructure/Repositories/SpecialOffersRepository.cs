using Microsoft.EntityFrameworkCore;
using WebSite.Domain.Interfaces;
using WebSite.Domain.Models;
using WebSite.Infrastructure.Database;

namespace WebSite.Infrastructure.Repositories
{
    public class SpecialOffersRepository : ISpecialOffersRepository
    {
        private readonly AppDbContext _context;

        public SpecialOffersRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SpecialOffer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SpecialOffers
                .OrderBy(o => o.Order)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SpecialOffer>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SpecialOffers
                .Where(o => o.IsActive)
                .OrderBy(o => o.Order)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<SpecialOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.SpecialOffers
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }
    }
}
