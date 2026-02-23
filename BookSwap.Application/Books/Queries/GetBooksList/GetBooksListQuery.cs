using MediatR;
using BookSwap.Application.Books.Dtos;

namespace BookSwap.Application.Books.Queries.GetBooksList
{
    public class GetBooksListQuery : IRequest<List<BookDto>>
    {
    }
}
