using MediatR;

namespace BookSwap.Application.Books.Commands.DeleteBook
{

    public class DeleteBookCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
    }

}
