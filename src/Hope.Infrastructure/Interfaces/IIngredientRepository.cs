using Hope.Domain.Models;

namespace Hope.Infrastructure.Interfaces
{
    public interface IIngredientRepository
    {
        public void Add(Ingredient ingredient);
        public Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken ct);
        public Task<IReadOnlyList<string>> GetAllAsync(CancellationToken ct);
        public Task<bool> ExistsByName(string name, CancellationToken ct);
    }
}
