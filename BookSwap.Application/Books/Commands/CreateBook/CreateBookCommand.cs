using MediatR;
using BookSwap.Application.Books.Dtos;

namespace BookSwap.Application.Books.Commands.CreateBook
{
    public class CreateBookCommand : IRequest<BookDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }
}
