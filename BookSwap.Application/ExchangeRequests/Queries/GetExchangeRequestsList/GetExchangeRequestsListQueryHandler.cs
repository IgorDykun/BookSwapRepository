using AutoMapper;
using BookSwap.Application.ExchangeRequests.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookSwap.Application.ExchangeRequests.Queries.GetExchangeRequestsList
{
    public class GetExchangeRequestsListQueryHandler : IRequestHandler<GetExchangeRequestsListQuery, List<ExchangeRequestDto>>
    {
        private readonly IExchangeRequestRepository _repository;
        private readonly IMapper _mapper;

        public GetExchangeRequestsListQueryHandler(IExchangeRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ExchangeRequestDto>> Handle(GetExchangeRequestsListQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<List<ExchangeRequestDto>>(entities);
        }
    }
}
