using MediatR;
using BookSwap.Application.Books.Dtos;

namespace BookSwap.Application.Books.Queries.GetBookById
{
    public class GetBookByIdQuery : IRequest<BookDto>
    {
        public string Id { get; set; } = string.Empty;
    }
}
