using BookSwap.Aggregator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using System.Net.Http.Json;

namespace BookSwap.Aggregator.Controllers
{
    [ApiController]
    [Route("api/aggregator/[controller]")]
    public class TransactionAggregatorController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AsyncPolicyWrap<HttpResponseMessage> _policy;

        public TransactionAggregatorController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> GetTransactionById(string id)
        {
            var client = _httpClientFactory.CreateClient("books-api");

            try
            {
                var response = await _policy.ExecuteAsync(() => client.GetAsync($"/api/mongo/TransactionMongo/{id}"));
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Transaction API temporarily unavailable");

                var transactionDto = await response.Content.ReadFromJsonAsync<dynamic>();

                var viewModel = new TransactionViewModel
                {
                    Id = transactionDto.id,
                    ExchangeRequestId = transactionDto.exchangeRequestId,
                    Status = transactionDto.user1Confirmed && transactionDto.user2Confirmed ? "Completed" : "Pending",
                    IsCompleted = transactionDto.user1Confirmed && transactionDto.user2Confirmed,
                    CompletedAt = transactionDto.completedAt
                };

                return Ok(viewModel);
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(503, "Service temporarily unavailable. Try again later.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var client = _httpClientFactory.CreateClient("books-api");

            try
            {
                var response = await _policy.ExecuteAsync(() => client.GetAsync("/api/mongo/TransactionMongo"));
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Transaction API temporarily unavailable");

                var transactionList = await response.Content.ReadFromJsonAsync<List<dynamic>>();

                var viewModels = transactionList.Select(t => new TransactionViewModel
                {
                    Id = t.id,
                    ExchangeRequestId = t.exchangeRequestId,
                    Status = t.user1Confirmed && t.user2Confirmed ? "Completed" : "Pending",
                    IsCompleted = t.user1Confirmed && t.user2Confirmed,
                    CompletedAt = t.completedAt
                }).ToList();

                return Ok(viewModels);
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(503, "Service temporarily unavailable. Try again later.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }
}
