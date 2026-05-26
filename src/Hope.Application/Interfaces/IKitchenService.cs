using FluentValidation.Results;
using Hope.Application.DTOs.Insert;

namespace Hope.Application.Interfaces
{
    public interface IKitchenService
    {
        public Task<ValidationResult> CreateAsync(IngredientInsertDto dtInsert, CancellationToken ct);
        public Task<IReadOnlyList<string>> GetAllIngredientsAsync(CancellationToken ct);
    }
}
