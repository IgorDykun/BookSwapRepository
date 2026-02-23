using BookSwap.Aggregator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using System.Net.Http.Json;
using OpenTelemetry.Trace;

namespace BookSwap.Aggregator.Controllers
{
    [ApiController]
    [Route("api/aggregator/[controller]")]
    public class ExchangeAggregatorController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AsyncPolicyWrap<HttpResponseMessage> _policy;

        public ExchangeAggregatorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            var retryPolicy = Policy.Handle<HttpRequestException>()
                                    .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                                    .RetryAsync(3);

            var circuitBreakerPolicy = Policy.Handle<HttpRequestException>()
                                             .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                                             .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

            _policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.GetExchangeById");
            span.SetAttribute("http.method", "GET");
            span.SetAttribute("http.route", $"/api/aggregator/ExchangeAggregator/{id}");

            var client = _httpClientFactory.CreateClient("books-api");
            try
            {
                var response = await _policy.ExecuteAsync(() => client.GetAsync($"/api/mongo/ExchangeMongo/{id}"));
                span.SetAttribute("http.status_code", (int)response.StatusCode);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Exchange API temporarily unavailable");

                var exchange = await response.Content.ReadFromJsonAsync<ExchangeRequestViewModel>();
                if (exchange == null) return NotFound();

                return Ok(exchange);
            }
            catch (BrokenCircuitException)
            {
                span.SetStatus(Status.Error.WithDescription("Circuit breaker triggered"));
                return StatusCode(503, "Service temporarily unavailable. Try again later.");
            }
            catch (Exception ex)
            {
                span.SetStatus(Status.Error.WithDescription(ex.Message));
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.GetAllExchanges");
            span.SetAttribute("http.method", "GET");
            span.SetAttribute("http.route", "/api/aggregator/ExchangeAggregator");

            var client = _httpClientFactory.CreateClient("books-api");
            try
            {
                var response = await _policy.ExecuteAsync(() => client.GetAsync("/api/mongo/ExchangeMongo"));
                span.SetAttribute("http.status_code", (int)response.StatusCode);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Exchange API temporarily unavailable");

                var list = await response.Content.ReadFromJsonAsync<List<ExchangeRequestViewModel>>();
                return Ok(list ?? new List<ExchangeRequestViewModel>());
            }
            catch (BrokenCircuitException)
            {
                span.SetStatus(Status.Error.WithDescription("Circuit breaker triggered"));
                return StatusCode(503, "Service temporarily unavailable. Try again later.");
            }
            catch (Exception ex)
            {
                span.SetStatus(Status.Error.WithDescription(ex.Message));
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExchangeRequestViewModel request)
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.CreateExchange");
            span.SetAttribute("http.method", "POST");
            span.SetAttribute("http.route", "/api/aggregator/ExchangeAggregator");

            var client = _httpClientFactory.CreateClient("books-api");
            try
            {
                var response = await _policy.ExecuteAsync(() => client.PostAsJsonAsync("/api/mongo/ExchangeMongo", request));
                span.SetAttribute("http.status_code", (int)response.StatusCode);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Exchange API temporarily unavailable");

                var created = await response.Content.ReadFromJsonAsync<ExchangeRequestViewModel>();
                return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
            }
            catch (BrokenCircuitException)
            {
                span.SetStatus(Status.Error.WithDescription("Circuit breaker triggered"));
                return StatusCode(503, "Service temporarily unavailable. Try again later.");
            }
            catch (Exception ex)
            {
                span.SetStatus(Status.Error.WithDescription(ex.Message));
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ExchangeRequestViewModel request)
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.UpdateExchange");
            span.SetAttribute("http.method", "PUT");
            span.SetAttribute("http.route", $"/api/aggregator/ExchangeAggregator/{id}");

            var client = _httpClientFactory.CreateClient("books-api");
            try
            {
                var response = await _policy.ExecuteAsync(() => client.PutAsJsonAsync($"/api/mongo/ExchangeMongo/{id}", request));
                span.SetAttribute("http.status_code", (int)response.StatusCode);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Exchange API temporarily unavailable");

                var updated = await response.Content.ReadFromJsonAsync<ExchangeRequestViewModel>();
                return Ok(updated);
            }
            catch (BrokenCircuitException)
            {
                span.SetStatus(Status.Error.WithDescription("Circuit breaker triggered"));
                return StatusCode(503, "Service temporarily unavailable. Try again later.");
            }
            catch (Exception ex)
            {
                span.SetStatus(Status.Error.WithDescription(ex.Message));
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.DeleteExchange");
            span.SetAttribute("http.method", "DELETE");
            span.SetAttribute("http.route", $"/api/aggregator/ExchangeAggregator/{id}");

            var client = _httpClientFactory.CreateClient("books-api");
            try
            {
                var response = await _policy.ExecuteAsync(() => client.DeleteAsync($"/api/mongo/ExchangeMongo/{id}"));
                span.SetAttribute("http.status_code", (int)response.StatusCode);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Exchange API temporarily unavailable");

                return NoContent();
            }
            catch (BrokenCircuitException)
            {
                span.SetStatus(Status.Error.WithDescription("Circuit breaker triggered"));
                return StatusCode(503, "Service temporarily unavailable. Try again later.");
            }
            catch (Exception ex)
            {
                span.SetStatus(Status.Error.WithDescription(ex.Message));
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpPut("{id}/confirm/{userId}")]
        public async Task<IActionResult> Confirm(string id, string userId)
        {
            var tracer = TracerProvider.Default.GetTracer("BookSwap.Aggregator");
            using var span = tracer.StartActiveSpan("Aggregator.ConfirmExchange");
            span.SetAttribute("http.method", "PUT");
            span.SetAttribute("http.route", $"/api/aggregator/ExchangeAggregator/{id}/confirm/{userId}");

            var client = _httpClientFactory.CreateClient("books-api");
            try
            {
                var response = await _policy.ExecuteAsync(() => client.PutAsync($"/api/mongo/ExchangeMongo/{id}/confirm/{userId}", null));
                span.SetAttribute("http.status_code", (int)response.StatusCode);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Exchange API temporarily unavailable");

                return NoContent();
            }
            catch (BrokenCircuitException)
            {
                span.SetStatus(Status.Error.WithDescription("Circuit breaker triggered"));
                return StatusCode(503, "Service temporarily unavailable. Try again later.");
            }
            catch (Exception ex)
            {
                span.SetStatus(Status.Error.WithDescription(ex.Message));
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }
}
