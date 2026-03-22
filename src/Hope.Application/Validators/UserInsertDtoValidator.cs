using FluentValidation;
using Hope.Application.DTOs.Insert;
using Hope.Infrastructure.Interfaces;

namespace Hope.Application.Validators
{
    public class UserInsertDtoValidator : AbstractValidator<UserInsertDto>
    {
        public UserInsertDtoValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Surname).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MustAsync( async (email, ct) => !await uow.UserRepository.ExistsByEmail(email, ct)).WithMessage("Invalid Email");
            RuleFor(x => x.PhoneNumber).NotEmpty().MustAsync(async (number, ct) => !await uow.UserRepository.ExistsByPhoneNumber(number, ct)).WithMessage("Invalid Phone");
        }
    }
}
