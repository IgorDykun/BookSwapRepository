using AutoMapper;
using BookSwap.Application.Books.Commands.CreateBook;
using BookSwap.Application.Books.Commands.DeleteBook;
using BookSwap.Application.Books.Commands.UpdateBook;
using BookSwap.Application.Books.Dtos;
using BookSwap.Application.Books.Queries.GetBookById;
using BookSwap.Application.Books.Queries.GetBooksList;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BookSwap.Api.Controllers.Mongo
{
    [ApiController]
    [Route("api/mongo/[controller]")]
    public class BookMongoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

        public BookMongoController(IMediator mediator, IDistributedCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookCommand command, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdBook = await _mediator.Send(command, cancellationToken);

            await _cache.RemoveAsync("books:all", cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, createdBook);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            var cacheKey = $"books:{id}";
            var cachedBook = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedBook))
            {
                var bookFromCache = JsonSerializer.Deserialize<BookDto>(cachedBook);
                return Ok(bookFromCache);
            }

            var query = new GetBookByIdQuery { Id = id };
            var book = await _mediator.Send(query, cancellationToken);
            if (book == null) return NotFound();

            var serialized = JsonSerializer.Serialize(book);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }, cancellationToken);

            return Ok(book);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var cacheKey = "books:all";
            var cachedBooks = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedBooks))
            {
                var booksFromCache = JsonSerializer.Deserialize<List<BookDto>>(cachedBooks);
                return Ok(booksFromCache);
            }

            var query = new GetBooksListQuery();
            var books = await _mediator.Send(query, cancellationToken);

            var serialized = JsonSerializer.Serialize(books);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }, cancellationToken);

            return Ok(books);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(string id, [FromBody] UpdateBookCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            try
            {
                var updatedBook = await _mediator.Send(command, cancellationToken);

                await _cache.RemoveAsync($"books:{id}", cancellationToken);
                await _cache.RemoveAsync("books:all", cancellationToken);

                return Ok(updatedBook);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(string id, CancellationToken cancellationToken)
        {
            var command = new DeleteBookCommand { Id = id };
            var deleted = await _mediator.Send(command, cancellationToken);

            if (!deleted) return NotFound();

            await _cache.RemoveAsync($"books:{id}", cancellationToken);
            await _cache.RemoveAsync("books:all", cancellationToken);

            return NoContent();
        }
    }
}
