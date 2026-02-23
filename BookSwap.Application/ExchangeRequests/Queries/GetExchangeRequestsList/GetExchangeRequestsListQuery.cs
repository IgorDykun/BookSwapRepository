using MediatR;
using BookSwap.Application.ExchangeRequests.Dtos;
using System.Collections.Generic;

namespace BookSwap.Application.ExchangeRequests.Queries.GetExchangeRequestsList
{
    public class GetExchangeRequestsListQuery : IRequest<List<ExchangeRequestDto>>
    {
    }
}
