using BookSwap.Aggregator.ViewModels;
using BookSwap.GrpcContracts;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BookSwap.Aggregator.Services.Grpc
{
    public class BooksGrpcClientService
    {
        private readonly BooksService.BooksServiceClient _grpcClient;
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public BooksGrpcClientService(BooksService.BooksServiceClient grpcClient, IDistributedCache cache)
        {
            _grpcClient = grpcClient;
            _cache = cache;
        }

        public async Task<BookViewModel?> GetBookByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"books:{id}";
            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<BookViewModel>(cached, _serializerOptions);

            try
            {
                var bookReply = await _grpcClient.GetBookByIdAsync(
                    new GetBookByIdRequest { Id = id },
                    cancellationToken: cancellationToken);

                if (bookReply == null || string.IsNullOrEmpty(bookReply.Id))
                    return null;

                var book = MapReplyToViewModel(bookReply);

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(book), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, cancellationToken);

                return book;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<BookViewModel>> GetAllBooksAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = "books:all";
            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<List<BookViewModel>>(cached, _serializerOptions)!;

            try
            {
                var booksReply = await _grpcClient.GetAllBooksAsync(
                    new GetAllBooksRequest(),
                    cancellationToken: cancellationToken);

                if (booksReply?.Books == null)
                    return new List<BookViewModel>();

                var books = booksReply.Books.Select(MapReplyToViewModel).ToList();

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(books), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, cancellationToken);

                return books;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return new List<BookViewModel>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<BookReply> CreateBookAsync(BookViewModel book, CancellationToken cancellationToken = default)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));

            var request = new CreateBookRequest
            {
                Title = book.Title,
                Author = book.Author,
                Owner = new UserSummaryRequest
                {
                    Id = book.Owner?.Id ?? string.Empty,
                    Name = book.Owner?.Name ?? string.Empty
                }
            };

            return await _grpcClient.CreateBookAsync(request, cancellationToken: cancellationToken);
        }

        public async Task<BookViewModel> UpdateBookAsync(string id, BookViewModel book, CancellationToken cancellationToken = default)
        {
            var reply = await _grpcClient.UpdateBookAsync(new UpdateBookRequest
            {
                Id = id,
                Title = book.Title,
                Author = book.Author
            }, cancellationToken: cancellationToken);

            await _cache.RemoveAsync($"books:{id}", cancellationToken);
            await _cache.RemoveAsync("books:all", cancellationToken);

            return MapReplyToViewModel(reply);
        }

        public async Task<bool> DeleteBookAsync(string id, CancellationToken cancellationToken = default)
        {
            var reply = await _grpcClient.DeleteBookAsync(new DeleteBookRequest { Id = id }, cancellationToken: cancellationToken);

            if (reply.Success)
            {
                await _cache.RemoveAsync($"books:{id}", cancellationToken);
                await _cache.RemoveAsync("books:all", cancellationToken);
            }

            return reply.Success;
        }

        private static BookViewModel MapReplyToViewModel(BookReply reply)
        {
            return new BookViewModel
            {
                Id = reply.Id,
                Title = reply.Title,
                Author = reply.Author,
                Owner = new UserSummaryViewModel
                {
                    Id = reply.Owner?.Id ?? string.Empty,
                    Name = reply.Owner?.Name ?? string.Empty
                }
            };
        }
    }
}
