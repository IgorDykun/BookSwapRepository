using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using Telegram.Bot;

namespace BookSwap.TelegramBot.Controllers;

[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    private readonly ITelegramBotClient _botClient;

    public BotController(IConfiguration configuration)
    {
        _botClient = new TelegramBotClient(configuration.GetValue<string>("TelegramToken"));
    }

    [HttpPost("send-notification")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        try
        {
            await _botClient.SendMessage(request.UserId, $"НАГАДУВАННЯ: {request.Message}");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public class NotificationRequest
    {
        public long UserId { get; set; }
        public string Message { get; set; }
    }
}