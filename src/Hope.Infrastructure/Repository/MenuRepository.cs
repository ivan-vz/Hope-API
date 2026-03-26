using Hope.Domain.Models;
using Hope.Infrastructure.Data;
using Hope.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hope.Infrastructure.Repository
{
    public class MenuRepository(ApplicationDbContext context) : IMenuRepository
    {
        public void Add(Menu menu) => context.Menus.Add(menu);

        public async Task<IReadOnlyList<Menu>> GetAllAsync(CancellationToken ct) => await context.Menus.Include(x => x.Meals).ThenInclude(m => m.Tags).ToListAsync(ct);

        public async Task<IReadOnlyList<Menu>> GetAllByDateAsync(DateOnly date, CancellationToken ct) => await context.Menus
            .Include(x => x.Meals)
            .ThenInclude(m => m.Tags)
            .Where(x => x.AvailableMonths
            .Contains(date))
            .ToListAsync(ct);

        public async Task<IReadOnlyList<Menu>> GetByTagsAsync(IEnumerable<string> tags, CancellationToken ct) => await context.Menus
            .Include(x => x.Meals)
            .ThenInclude(m => m.Tags)
            .Where(x => x.Meals
            .Any(m => m.Tags
            .Any(t => tags.Contains(t.Name))))
            .ToListAsync(ct);

        public async Task<Menu?> GetByIdAsync(Guid id, CancellationToken ct) => await context.Menus.Include(x => x.Meals).ThenInclude(m => m.Tags).SingleOrDefaultAsync(x => x.Id == id, ct);

        public async Task<bool> ExistsByName(string name, CancellationToken ct)
        {
            var normalized = name.Trim().ToLowerInvariant();
            return await context.Menus.AnyAsync(x => x.Name.ToLower() == normalized, ct);
        }
    }
}
