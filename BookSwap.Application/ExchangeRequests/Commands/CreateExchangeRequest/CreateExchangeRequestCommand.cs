using MediatR;
using BookSwap.Application.ExchangeRequests.Dtos;

namespace BookSwap.Application.ExchangeRequests.Commands.CreateExchangeRequest
{
    public class CreateExchangeRequestCommand : IRequest<ExchangeRequestDto>
    {
        public string FromUserId { get; set; }
        public string FromUserName { get; set; } = string.Empty;
        public string ToUserId { get; set; }
        public string ToUserName { get; set; } = string.Empty;
        public string BookOfferedId { get; set; }
        public string BookOfferedTitle { get; set; } = string.Empty;
        public string BookRequestedId { get; set; }
        public string BookRequestedTitle { get; set; } = string.Empty;
    }
}
