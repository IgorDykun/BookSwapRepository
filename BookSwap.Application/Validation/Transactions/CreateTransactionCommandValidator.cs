using FluentValidation;
using BookSwap.Application.Transactions.Commands.CreateTransaction;
using BookSwap.Application.Validators;

namespace BookSwap.Application.Validators.Transactions
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.ExchangeRequestId).ValidObjectId();
        }
    }
}
