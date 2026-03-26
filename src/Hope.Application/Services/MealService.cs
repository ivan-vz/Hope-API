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
    public class MealService(IUnitOfWork uow, IValidator<MealInsertDto> insertValidator, IValidator<MealUpdateDto> updateValidator) : IMealService
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IValidator<MealInsertDto> _insertValidator = insertValidator;
        private readonly IValidator<MealUpdateDto> _updateValidator = updateValidator;

        public async Task<(MealDto?, ValidationResult)> CreateAsync(MealInsertDto dtInsert, CancellationToken ct)
        {
            var validation = await _insertValidator.ValidateAsync(dtInsert, ct);
            if (!validation.IsValid) return (null, validation);

            var meal = new Meal(dtInsert.Name, dtInsert.Description, dtInsert.Price);

            foreach (var tagName in dtInsert.Tags.Distinct())
            {
                var tag = await _uow.TagRepository.GetByNameAsync(tagName, ct);
                if (tag is null)
                {
                    validation.Errors.Add(new ValidationFailure
                        (
                        "TagName", $"Tag named {tagName} not found"
                        ));
                    continue;
                }

                meal.Tags.Add(tag);
            }

            if (!validation.IsValid) return (null, validation);

            _uow.MealRepository.Add(meal);
            await _uow.Complete();

            return (meal.ToDto(), validation);
        }

        public async Task<ValidationResult> DeleteAsync(Guid id, CancellationToken ct)
        {
            var validation = new ValidationResult();

            var meal = await _uow.MealRepository.GetByIdAsync(id, ct);
            if (meal is null)
            {
                validation.Errors.Add
                    (
                        new ValidationFailure("Id", "Meal not found")
                    );
                return validation;
            }

            meal.IsDeleted = true;
            await _uow.Complete();

            return validation;
        }

        public async Task<IReadOnlyList<MealDto>> GetAllAsync(CancellationToken ct) => [.. (await _uow.MealRepository.GetAllAsync(ct)).Select(x => x.ToDto())]; 

        public async Task<MealDto?> GetByIdAsync(Guid id, CancellationToken ct) => (await _uow.MealRepository.GetByIdAsync(id, ct))?.ToDto();

        public async Task<IReadOnlyList<MealDto>> GetByTagsAsync(IEnumerable<string> tags, CancellationToken ct) => [.. (await _uow.MealRepository.GetByTagsAsync(tags, ct)).Select(x => x.ToDto())];

        public async Task<(MealDto?, ValidationResult)> UpdateAsync(MealUpdateDto dtUpdate, CancellationToken ct)
        {
            var validation = await _updateValidator.ValidateAsync(dtUpdate, ct);
            if (!validation.IsValid) return (null, validation);

            var meal = await _uow.MealRepository.GetByIdAsync(dtUpdate.Id, ct);
            if (meal is null)
            {
                validation.Errors.Add(new ValidationFailure("Id", "Meal not found"));
                return (null, validation);
            }

            if (!string.IsNullOrEmpty(dtUpdate.Description)) meal.Description = dtUpdate.Description;
            if (!string.IsNullOrEmpty(dtUpdate.ImageUrl)) meal.ImageUrl = dtUpdate.ImageUrl;
            meal.Price = dtUpdate.Price;

            if (dtUpdate.Tags is not null) 
            {
                meal.Tags.Clear();

                foreach (var tagName in dtUpdate.Tags.Distinct())
                {
                    var tag = await _uow.TagRepository.GetByNameAsync(tagName, ct);
                    if (tag is null)
                    {
                        validation.Errors.Add(new ValidationFailure
                            (
                            "TagName", $"Tag named {tagName} not found"
                            ));
                        continue;
                    }

                    meal.Tags.Add(tag);
                }
                
                if (!validation.IsValid) return (null, validation);
            }

            await _uow.Complete();

            return (meal.ToDto(), validation);
        }
    }
}
