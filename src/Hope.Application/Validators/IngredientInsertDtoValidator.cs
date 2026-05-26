using FluentValidation;
using Hope.Application.DTOs.Insert;
using Hope.Infrastructure.Interfaces;

namespace Hope.Application.Validators
{
    public class IngredientInsertDtoValidator : AbstractValidator<IngredientInsertDto>
    {
        public IngredientInsertDtoValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MustAsync(async (name, ct) => !await uow.IngredientRepository.ExistsByName(name, ct)).WithMessage("Invalid name");
            RuleFor(x => x.IsLiquid).NotEmpty();
        }
    }
}
