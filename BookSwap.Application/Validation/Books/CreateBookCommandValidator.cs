using FluentValidation;
using BookSwap.Application.Books.Commands.CreateBook;
using BookSwap.Application.Validators;

namespace BookSwap.Application.Validators.Books
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(x => x.Title).NotEmptyString("Назва книги");
            RuleFor(x => x.Author).NotEmptyString("Автор");
            RuleFor(x => x.OwnerId).ValidObjectId();
        }
    }
}
