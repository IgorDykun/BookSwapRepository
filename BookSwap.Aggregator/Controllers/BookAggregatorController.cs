using BookSwap.Aggregator.Services;
using BookSwap.Aggregator.ViewModels;
using BookSwap.Domain.Entities;   // Виправлено: Entities замість Models
using BookSwap.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace BookSwap.Aggregator.Controllers
{
    [ApiController]
    [Route("api/aggregator/[controller]")]
    public class BookAggregatorController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly RedisStreamPublisher _redisPublisher;

        public BookAggregatorController(IBookRepository bookRepository, RedisStreamPublisher redisPublisher)
        {
            _bookRepository = bookRepository;
            _redisPublisher = redisPublisher;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.GetBookById");

            var book = await _bookRepository.GetByIdAsync(id);
            if (book is null) return NotFound();

            return Ok(book);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.GetAllBooks");

            var books = await _bookRepository.GetAllAsync();
            // Виправлено: BookDocument замість Book
            return Ok(books ?? new List<BookDocument>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookDocument book) // Виправлено: BookDocument
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.CreateBook");

            await _bookRepository.CreateAsync(book);

            // Публікація в Redis
            var bookVm = new BookViewModel
            {
                Id = book.Id ?? string.Empty,
                Title = book.Title,
                Author = book.Author,
                Owner = new UserSummaryViewModel
                {
                    Id = book.Owner.Id,
                    Name = book.Owner.Name
                }
            };
            await _redisPublisher.PublishBookCreatedAsync(bookVm);

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BookDocument book) // Виправлено: BookDocument
        {
            var existing = await _bookRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            book.Id = id; // Гарантуємо, що ID з URL переходить в об'єкт
            await _bookRepository.UpdateAsync(id, book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _bookRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _bookRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}