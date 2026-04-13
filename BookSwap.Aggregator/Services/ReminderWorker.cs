using BookSwap.Aggregator.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace BookSwap.Aggregator.Services;

public class ReminderWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReminderWorker> _logger;

    public ReminderWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ReminderWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Фоновий сервіс нагадувань запущено.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

                    var now = DateTime.Now;

                    var pendingReminders = await context.Reminders
                        .Where(r => !r.IsSent && r.NotifyAt <= now)
                        .ToListAsync(stoppingToken);

                    foreach (var reminder in pendingReminders)
                    {
                        _logger.LogInformation($"Час настав для нагадування ID: {reminder.Id}");

                        var client = httpClientFactory.CreateClient();

                        var botUrl = _configuration.GetValue<string>("BotUrl") ?? "http://localhost:5123";

                        var payload = new { UserId = reminder.UserId, Message = reminder.Text };

                        var response = await client.PostAsJsonAsync($"{botUrl}/api/bot/send-notification", payload, stoppingToken);

                        if (response.IsSuccessStatusCode)
                        {
                            reminder.IsSent = true;
                            _logger.LogInformation($"Нагадування для {reminder.UserId} успішно відправлено.");
                        }
                        else
                        {
                            _logger.LogError($"Не вдалося відправити нагадування боту. Код: {response.StatusCode}");
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка у фоновому сервісі нагадувань");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}