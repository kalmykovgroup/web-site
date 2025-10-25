using Microsoft.EntityFrameworkCore;
using WebSite.Domain.Interfaces;
using WebSite.Domain.Models;
using WebSite.Infrastructure.Database;

namespace WebSite.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories.Include(c => c.Images).OrderBy(c => c.Order)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

         
    }
}
