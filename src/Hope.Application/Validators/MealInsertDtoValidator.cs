using FluentValidation;
using Hope.Application.DTOs.Insert;
using Hope.Infrastructure.Interfaces;

namespace Hope.Application.Validators
{
    public class MealInsertDtoValidator : AbstractValidator<MealInsertDto>
    {
        public MealInsertDtoValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60).MustAsync(async (name, ct) => !await uow.MealRepository.ExistsByName(name, ct)).WithMessage("Invalid name");
            RuleFor(x => x.Description).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Price).NotNull().GreaterThanOrEqualTo(0).PrecisionScale(10, 2, false);
            RuleFor(x => x.Ingredients).Must(x => x.DistinctBy(i => i.Id).Count() == x.Count());
            RuleForEach(x => x.Ingredients).Must(x => x.Id != Guid.Empty).WithMessage("Id cannot be empty").Must(x => x.Quantity > 0).WithMessage("Quantity must be greater than 0");
        }
    }
}