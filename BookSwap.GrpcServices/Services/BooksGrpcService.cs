using BookSwap.GrpcContracts;
using Grpc.Core;
using MediatR;
using BookSwap.Application.Books.Commands.CreateBook;
using BookSwap.Application.Books.Commands.UpdateBook;
using BookSwap.Application.Books.Commands.DeleteBook;
using BookSwap.Application.Books.Queries.GetBooksList;
using BookSwap.Application.Books.Queries.GetBookById;
using BookSwap.Application.Books.Dtos; // <- DTO з Owner

namespace BookSwap.GrpcServices.Services
{
    public class BooksGrpcService : BooksService.BooksServiceBase
    {
        private readonly IMediator _mediator;

        public BooksGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<BookReply> GetBookById(GetBookByIdRequest request, ServerCallContext context)
        {
            var book = await _mediator.Send(new GetBookByIdQuery { Id = request.Id });

            if (book is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));

            return MapBookDtoToReply(book);
        }

        public override async Task<BooksListReply> GetAllBooks(GetAllBooksRequest request, ServerCallContext context)
        {
            var books = await _mediator.Send(new GetBooksListQuery());

            var reply = new BooksListReply();
            reply.Books.AddRange(books.Select(MapBookDtoToReply));

            return reply;
        }

        public override async Task<BookReply> CreateBook(CreateBookRequest request, ServerCallContext context)
        {
            if (request.Owner == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Owner cannot be null"));

            var created = await _mediator.Send(new CreateBookCommand
            {
                Title = request.Title,
                Author = request.Author,
                OwnerId = request.Owner.Id,
                OwnerName = request.Owner.Name
            });

            return new BookReply
            {
                Id = created.Id,
                Title = created.Title,
                Author = created.Author,
                Owner = new UserSummaryReply
                {
                    Id = created.Owner?.Id ?? string.Empty,
                    Name = created.Owner?.Name ?? string.Empty
                }
            };
        }

        public override async Task<BookReply> UpdateBook(UpdateBookRequest req, ServerCallContext context)
        {
            var updated = await _mediator.Send(new UpdateBookCommand
            {
                Id = req.Id,
                Title = req.Title,
                Author = req.Author
            });

            return new BookReply
            {
                Id = updated.Id,
                Title = updated.Title,
                Author = updated.Author,
                Owner = new UserSummaryReply
                {
                    Id = updated.Owner.Id,
                    Name = updated.Owner.Name
                }
            };
        }
        public override async Task<DeleteBookReply> DeleteBook(DeleteBookRequest request, ServerCallContext context)
        {
            var success = await _mediator.Send(new DeleteBookCommand { Id = request.Id });
            return new DeleteBookReply { Success = success };
        }

        private static BookReply MapBookDtoToReply(BookDto dto)
        {
            return new BookReply
            {
                Id = dto.Id,
                Title = dto.Title,
                Author = dto.Author,
                Owner = new UserSummaryReply
                {
                    Id = dto.Owner?.Id ?? string.Empty,
                    Name = dto.Owner?.Name ?? string.Empty
                }
            };
        }
    }
}
