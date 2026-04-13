using BookSwap.Aggregator.Data;
using BookSwap.Aggregator.Models;
using BookSwap.Aggregator.Services; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OpenTelemetry.Trace;
using System.Text.Json;
using NewtonsoftJson = Newtonsoft.Json.JsonConvert;

namespace BookSwap.Aggregator.Controllers
{
    [ApiController]
    [Route("api/aggregator/[controller]")]
    public class ExternalDataController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ExternalDataController> _logger;
        private readonly AppDbContext _context;
        private readonly ReminderParser _parser;
        private readonly CalendarManager _calendarManager;

        public ExternalDataController(
            IHttpClientFactory httpClientFactory,
            IDistributedCache cache,
            ILogger<ExternalDataController> logger,
            AppDbContext context,
            ReminderParser parser,
            CalendarManager calendarManager)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
            _context = context;
            _parser = parser;
            _calendarManager = calendarManager;
        }

        [HttpGet("search-books/{title}")]
        public async Task<IActionResult> SearchBooks(string title, CancellationToken cancellationToken)
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.ExternalSearch");

            string cacheKey = $"books_search_{title.ToLower().Replace(" ", "_")}";

            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return Ok(JsonSerializer.Deserialize<object>(cachedData));
                }

                var client = _httpClientFactory.CreateClient();
                var openLibraryUrl = $"https://openlibrary.org/search.json?title={Uri.EscapeDataString(title)}&limit=5";

                var response = await client.GetAsync(openLibraryUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Open Library повернув помилку: {StatusCode}", response.StatusCode);
                    return Ok(new { items = new List<object>(), message = "Сервіс пошуку книг тимчасово недоступний." });
                }

                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
                dynamic openLibData = NewtonsoftJson.DeserializeObject(jsonString);

                var formattedItems = new List<object>();
                if (openLibData?.docs != null)
                {
                    foreach (var doc in openLibData.docs)
                    {
                        formattedItems.Add(new
                        {
                            volumeInfo = new { title = (string)doc.title }
                        });
                    }
                }

                var finalResult = new { items = formattedItems };
                var serializedResult = JsonSerializer.Serialize(finalResult);

                await _cache.SetStringAsync(cacheKey, serializedResult, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, cancellationToken);

                return Ok(finalResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка пошуку для: {Title}", title);
                return Ok(new { items = new List<object>(), message = "Сталася технічна помилка при пошуку." });
            }
        }

        [HttpPost("process-message")]
        public async Task<IActionResult> ProcessMessage([FromBody] UserRequest request, CancellationToken ct)
        {
            var profile = await _context.UserProfiles.FindAsync(request.UserId);
            if (profile == null)
            {
                profile = new UserProfile { UserId = request.UserId, Name = request.UserName ?? "Користувач" };
                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Створено новий профіль для ID: {Id}", request.UserId);
            }

            if (request.Message.Contains("створи подію", StringComparison.OrdinalIgnoreCase) ||
                request.Message.Contains("в календар", StringComparison.OrdinalIgnoreCase))
            {
                await _calendarManager.AuthenticateAsync(); 
                string resultMessage = await _calendarManager.CreateEventFromTextAsync(request.Message);
                return Ok(new { message = resultMessage });
            }

            if (request.Message.Contains("мої події", StringComparison.OrdinalIgnoreCase) ||
                request.Message.Contains("найближчі події", StringComparison.OrdinalIgnoreCase))
            {
                await _calendarManager.AuthenticateAsync(); 
                string resultMessage = await _calendarManager.GetUpcomingEventsAsync(5);
                return Ok(new { message = resultMessage });
            }

            if (request.Message.Contains("нагадай", StringComparison.OrdinalIgnoreCase))
            {
                var (time, text) = _parser.Parse(request.Message);

                if (time.HasValue)
                {
                    var reminder = new Reminder
                    {
                        UserId = request.UserId,
                        Text = text ?? "Без тексту",
                        NotifyAt = time.Value,
                        IsSent = false
                    };

                    _context.Reminders.Add(reminder);
                    await _context.SaveChangesAsync(ct);

                    return Ok(new { message = $"Зрозумів! Нагадаю о {time.Value:HH:mm, dd MMMM}." });
                }

                return Ok(new { message = "Я зрозумів, що це нагадування, але не впізнав час. Напиши, наприклад: 'Нагадай завтра о 15:00 купити книгу'" });
            }

            var client = _httpClientFactory.CreateClient();
            try
            {
                var nlpResponse = await client.PostAsJsonAsync("http://localhost:8000/analyze", new { text = request.Message }, ct);
                var rawJson = await nlpResponse.Content.ReadAsStringAsync(ct);
                var analysis = JsonSerializer.Deserialize<NlpResponse>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (analysis?.Intent == "weather_request")
                {
                    string cityInfo = string.IsNullOrEmpty(profile.FavoriteCity)
                        ? "ви не вказали улюблене місто"
                        : $"у місті {profile.FavoriteCity}";

                    return Ok(new { message = $"Привіт, {profile.Name}! Погода {cityInfo} зараз чудова для читання книг." });
                }

                return await SearchBooks(request.Message, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError("Помилка обробки: {Msg}", ex.Message);
                return await SearchBooks(request.Message, ct);
            }
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdate update)
        {
            var profile = await _context.UserProfiles.FindAsync(update.UserId);
            if (profile == null)
            {
                profile = new UserProfile { UserId = update.UserId, Name = update.Name ?? "Користувач" };
                _context.UserProfiles.Add(profile);
            }

            if (!string.IsNullOrEmpty(update.FavoriteCity))
            {
                profile.FavoriteCity = update.FavoriteCity;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Профіль оновлено" });
        }

        public class UserProfileUpdate
        {
            [System.Text.Json.Serialization.JsonPropertyName("userId")]
            public long UserId { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("favoriteCity")]
            public string FavoriteCity { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class NlpResponse
        {
            public string Intent { get; set; }
            public List<NlpEntity> Entities { get; set; }
        }

        public class NlpEntity
        {
            public string Text { get; set; }
            public string Label { get; set; }
        }

        public class UserRequest
        {
            public long UserId { get; set; }
            public string Message { get; set; }
            public string UserName { get; set; }
        }
    }
}