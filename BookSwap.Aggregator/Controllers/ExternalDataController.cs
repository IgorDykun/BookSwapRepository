using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using OpenTelemetry.Trace;

namespace BookSwap.Aggregator.Controllers
{
    [ApiController]
    [Route("api/aggregator/[controller]")]
    public class ExternalDataController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ExternalDataController> _logger;

        public ExternalDataController(IHttpClientFactory httpClientFactory, IDistributedCache cache, ILogger<ExternalDataController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("search-google-books/{title}")]
        public async Task<IActionResult> SearchBooks(string title, CancellationToken cancellationToken)
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.ExternalSearch");

            string cacheKey = $"google_books_{title}";

            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("Дані для '{Title}' отримано з кешу Redis", title);
                    return Ok(JsonSerializer.Deserialize<object>(cachedData));
                }

                _logger.LogInformation("Запит до Google Books API для: {Title}", title);
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://www.googleapis.com/books/v1/volumes?q={title}", cancellationToken);

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

                await _cache.SetStringAsync(cacheKey, jsonString, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, cancellationToken);

                return Ok(JsonSerializer.Deserialize<object>(jsonString));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка зовнішнього API для запиту: {Title}", title);
                return StatusCode(500, new { message = "Зовнішній сервіс тимчасово недоступний", error = ex.Message });
            }
        }
    }
}