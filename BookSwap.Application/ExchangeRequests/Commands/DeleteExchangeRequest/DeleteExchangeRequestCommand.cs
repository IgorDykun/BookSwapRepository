using MediatR;

namespace BookSwap.Application.ExchangeRequests.Commands.DeleteExchangeRequest
{
    public class DeleteExchangeRequestCommand : IRequest<Unit>
    {
        public string Id { get; set; } = string.Empty;
    }
}
