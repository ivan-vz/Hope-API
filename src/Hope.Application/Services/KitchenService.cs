using FluentValidation;
using FluentValidation.Results;
using Hope.Application.DTOs.Insert;
using Hope.Application.Interfaces;
using Hope.Domain.Models;
using Hope.Infrastructure.Interfaces;

namespace Hope.Application.Services
{
    public class KitchenService(IUnitOfWork uow, IValidator<IngredientInsertDto> validator) : IKitchenService
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IValidator<IngredientInsertDto> _validator = validator;

        public async Task<ValidationResult> CreateAsync(IngredientInsertDto dtInsert, CancellationToken ct)
        {
            var validation = await _validator.ValidateAsync(dtInsert, ct);
            if (!validation.IsValid) return validation;

            var instance = new Ingredient(dtInsert.Name, dtInsert.IsLiquid);

            _uow.IngredientRepository.Add(instance);
            await _uow.Complete();

            return validation;
        }

        public async Task<IReadOnlyList<string>> GetAllIngredientsAsync(CancellationToken ct) => await _uow.IngredientRepository.GetAllAsync(ct);
    }
}
