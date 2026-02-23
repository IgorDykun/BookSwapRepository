using MediatR;
using BookSwap.Application.ExchangeRequests.Dtos;

namespace BookSwap.Application.ExchangeRequests.Commands.UpdateExchangeRequest
{
    public class UpdateExchangeRequestCommand : IRequest<ExchangeRequestDto>
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
