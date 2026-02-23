using MediatR;
using BookSwap.Application.ExchangeRequests.Dtos;

namespace BookSwap.Application.ExchangeRequests.Queries.GetExchangeRequestById
{
    public class GetExchangeRequestByIdQuery : IRequest<ExchangeRequestDto>
    {
        public string Id { get; set; } = string.Empty;
    }
}
