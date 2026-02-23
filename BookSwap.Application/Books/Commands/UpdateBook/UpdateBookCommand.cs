using MediatR;
using BookSwap.Application.Books.Dtos;

namespace BookSwap.Application.Books.Commands.UpdateBook
{
    public class UpdateBookCommand : IRequest<BookDto>
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}
