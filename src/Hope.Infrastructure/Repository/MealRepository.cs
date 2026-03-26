using Hope.Domain.Models;
using Hope.Infrastructure.Data;
using Hope.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hope.Infrastructure.Repository
{
    internal class MealRepository(ApplicationDbContext context) : IMealRepository
    {
        public void Add(Meal meal) => context.Meals.Add(meal);

        public async Task<bool> ExistsByName(string name, CancellationToken ct)
        {
            var normalized = name.Trim().ToLowerInvariant();
            return await context.Meals.AnyAsync(x => x.Name.ToLower() == normalized, ct);
        }

        public async Task<IReadOnlyList<Meal>> GetAllAsync(CancellationToken ct) => await context.Meals.Include(x => x.Tags).ToListAsync(ct);

        public async Task<Meal?> GetByIdAsync(Guid id, CancellationToken ct) => await context.Meals.Include(x => x.Tags).SingleOrDefaultAsync(x => x.Id == id, ct);

        public async Task<IReadOnlyList<Meal>> GetByTagsAsync(IEnumerable<string> tags, CancellationToken ct)
        {
            var tagList = tags.ToList();

            return await context.Meals
            .Include(x => x.Tags)
            .Where(x => x.Tags
            .Any(t => tags.Contains(t.Name)))
            .ToListAsync(ct);
        }
    }
}
