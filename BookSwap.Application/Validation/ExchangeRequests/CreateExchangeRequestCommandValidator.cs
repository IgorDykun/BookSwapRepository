using FluentValidation;
using BookSwap.Application.ExchangeRequests.Commands.CreateExchangeRequest;
using BookSwap.Application.Validators;

namespace BookSwap.Application.Validators.ExchangeRequests
{
    public class CreateExchangeRequestCommandValidator : AbstractValidator<CreateExchangeRequestCommand>
    {
        public CreateExchangeRequestCommandValidator()
        {
            RuleFor(x => x.FromUserId).ValidObjectId();
            RuleFor(x => x.ToUserId).ValidObjectId();
            RuleFor(x => x.BookOfferedId).ValidObjectId();
            RuleFor(x => x.BookRequestedId).ValidObjectId();

            RuleFor(x => x.FromUserName).NotEmptyString("Ім’я користувача-відправника");
            RuleFor(x => x.ToUserName).NotEmptyString("Ім’я користувача-отримувача");
        }
    }
}
