using System.Text;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BookSwap.TelegramBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITelegramBotClient _botClient;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _botClient = new TelegramBotClient(_configuration.GetValue<string>("TelegramToken"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botClient.StartReceiving(
            updateHandler: async (bot, update, ct) => await HandleUpdateAsync(bot, update, ct),
            errorHandler: async (bot, ex, ct) => await Task.CompletedTask,
            receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Message is not { Text: { } messageText } message) return;

        var chatId = message.Chat.Id;

        if (messageText.ToLower() == "/start")
        {
            await botClient.SendMessage(chatId, "Привіт! Я розумію звичайну мову. Просто напиши, що ти шукаєш.", cancellationToken: ct);
            return;
        }
        if (messageText.StartsWith("/setcity"))
        {
            var city = messageText.Replace("/setcity", "").Trim();
            if (string.IsNullOrEmpty(city))
            {
                await botClient.SendMessage(chatId, "Будь ласка, вкажіть місто. Приклад: /setcity Київ", cancellationToken: ct);
                return;
            }

            var baseUrl = _configuration.GetValue<string>("AggregatorUrl");
            var client = _httpClientFactory.CreateClient();
            var payload = new { UserId = chatId, FavoriteCity = city, Name = message.From?.FirstName };

            var response = await client.PostAsJsonAsync($"{baseUrl}/api/aggregator/ExternalData/update-profile", payload, ct);
            if (response.IsSuccessStatusCode)
                await botClient.SendMessage(chatId, $"Місто {city} збережено!", cancellationToken: ct);
            else
                await botClient.SendMessage(chatId, "Не вдалося зберегти місто.", cancellationToken: ct);

            return;
        }

        string firstName = message.From?.FirstName ?? "Друже";
        await ProcessViaNlpAggregator(chatId, firstName, messageText, ct);
    }

    private async Task ProcessViaNlpAggregator(long chatId, string username, string query, CancellationToken ct)
    {
        try
        {
            var baseUrl = _configuration.GetValue<string>("AggregatorUrl");
            var client = _httpClientFactory.CreateClient();
            var payload = new { UserId = chatId, Message = query, UserName = username };
            var response = await client.PostAsJsonAsync($"{baseUrl}/api/aggregator/ExternalData/process-message", payload, ct);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                dynamic result = JsonConvert.DeserializeObject(content);

                if (result?.message != null)
                {
                    await _botClient.SendMessage(chatId, (string)result.message, cancellationToken: ct);
                }
                else if (result?.items != null)
                {
                    var sb = new StringBuilder($"Ось що я знайшов:\n\n");
                    foreach (var item in result.items)
                    {
                        sb.AppendLine($"- {item.volumeInfo.title}");
                    }
                    await _botClient.SendMessage(chatId, sb.ToString(), cancellationToken: ct);
                }
            }
            else
            {
                await _botClient.SendMessage(chatId, "Помилка Агрегатора.", cancellationToken: ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            await _botClient.SendMessage(chatId, "Сталася помилка.", cancellationToken: ct);
        }
    }
}