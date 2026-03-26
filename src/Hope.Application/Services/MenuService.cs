using FluentValidation;
using FluentValidation.Results;
using Hope.Application.DTOs.Detail;
using Hope.Application.DTOs.Insert;
using Hope.Application.DTOs.Update;
using Hope.Application.Extensions;
using Hope.Application.Interfaces;
using Hope.Domain.Models;
using Hope.Infrastructure.Interfaces;

namespace Hope.Application.Services
{
    public class MenuService(IUnitOfWork uow, IValidator<MenuInsertDto> insertValidator, IValidator<MenuUpdateDto> updateValidator) : IMenuService
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IValidator<MenuInsertDto> _insertValidator = insertValidator;
        private readonly IValidator<MenuUpdateDto> _updateValidator = updateValidator;

        public async Task<(MenuDto?, ValidationResult)> CreateAsync(MenuInsertDto dtInsert, CancellationToken ct)
        {
            var validation = await _insertValidator.ValidateAsync(dtInsert, ct);
            if (!validation.IsValid) return (null, validation);

            var meals = new List<Meal>();
            foreach (var mealId in dtInsert.Meals.Distinct()) 
            {
                var meal = await _uow.MealRepository.GetByIdAsync(mealId, ct);
                if(meal is null)
                {
                    validation.Errors.Add(new ValidationFailure("Meal", $"Meal {mealId} not found for menu"));
                    continue;
                }

                meals.Add(meal);
            }

            if (!validation.IsValid) return (null, validation);

            var menu = new Menu(dtInsert.Name);
            foreach (var meal in meals) menu.Meals.Add(meal);

            _uow.MenuRepository.Add(menu);
            await _uow.Complete();

            return (menu.ToDto(), validation);
        }

        public async Task<ValidationResult> DeleteAsync(Guid id, CancellationToken ct)
        {
            var validation = new ValidationResult();

            var menu = await _uow.MenuRepository.GetByIdAsync(id, ct);
            if (menu is null)
            {
                validation.Errors.Add
                    (
                        new ValidationFailure("Id", "Menu not found")
                    );
                return validation;
            }

            menu.IsDeleted = true;
            await _uow.Complete();

            return validation;
        }

        public async Task<IReadOnlyList<MenuDto>> GetAllAsync(CancellationToken ct) => [.. (await _uow.MenuRepository.GetAllAsync(ct)).Select(x => x.ToDto())];

        public async Task<IReadOnlyList<MenuDto>> GetAllByDateAsync(DateOnly date, CancellationToken ct) => 
            [.. (await _uow.MenuRepository.GetAllByDateAsync(date, ct)).Select(x => x.ToDto())];

        public async Task<MenuDto?> GetByIdAsync(Guid id, CancellationToken ct) => (await _uow.MenuRepository.GetByIdAsync(id, ct))?.ToDto();

        public async Task<IReadOnlyList<MenuDto>> GetAllByTagsAsync(IEnumerable<string> tags, CancellationToken ct) => 
            [.. (await _uow.MenuRepository.GetByTagsAsync(tags, ct)).Select(x => x.ToDto())];

        public async Task<(MenuDto?, ValidationResult)> UpdateAsync(MenuUpdateDto dtUpdate, CancellationToken ct)
        {
            var validation = await _updateValidator.ValidateAsync(dtUpdate, ct);
            if (!validation.IsValid) return (null, validation);

            var menu = await _uow.MenuRepository.GetByIdAsync(dtUpdate.Id, ct);
            if (menu is null)
            {
                validation.Errors.Add
                    (
                        new ValidationFailure("Id", "Menu not found")
                    );
                return (null, validation);
            }

            var newMeals = new List<Meal>();
            foreach (var mealId in dtUpdate.Meals)
            {
                var meal = await _uow.MealRepository.GetByIdAsync(mealId, ct);
                if (meal is null)
                {
                    validation.Errors.Add
                        (
                            new ValidationFailure("Meal", $"Meal id {mealId} not found for menu")
                        );
                    continue;
                }

                newMeals.Add(meal);
            }

            if (!validation.IsValid) return (null, validation);

            menu.Meals.Clear();
            foreach (var meal in newMeals) menu.Meals.Add(meal);
            
            menu.AvailableMonths.Clear();
            foreach(var availableMonths in dtUpdate.AvailableMonths) menu.AvailableMonths.Add(availableMonths);

            await _uow.Complete();

            return (menu.ToDto(), validation);
        }
    }
}
