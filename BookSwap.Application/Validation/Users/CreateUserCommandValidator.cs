using FluentValidation;
using BookSwap.Application.Users.Commands.CreateUser;
using BookSwap.Application.Validators;

namespace BookSwap.Application.Validators.Users
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Name).NotEmptyString("Ім’я користувача");
            RuleFor(x => x.Email).ValidEmail();
        }
    }
}
