using MediatR;

namespace BookSwap.Application.ExchangeRequests.Commands.ConfirmExchangeRequest
{
    public class ConfirmExchangeRequestCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
