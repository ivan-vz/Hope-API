using Hope.Domain.Models;
using Hope.Infrastructure.Data;
using Hope.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hope.Infrastructure.Repository
{
    public class IngredientRepository(ApplicationDbContext context) : IIngredientRepository
    {
        public void Add(Ingredient ingredient) => context.Ingredients.Add(ingredient);

        public async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken ct) => await context.Ingredients.SingleOrDefaultAsync(c => c.Id == id, ct);

        public async Task<bool> ExistsByName(string name, CancellationToken ct)
        {
            var normalized = name.Trim().ToLowerInvariant();
            return await context.Ingredients.AnyAsync(x => x.Name.ToLower() == normalized, ct);
        }

        public async Task<IReadOnlyList<string>> GetAllAsync(CancellationToken ct) => await context.Ingredients.Select(x => x.Name).ToListAsync(ct);
    }
}
