using AutoMapper;
using BookSwap.Application.ExchangeRequests.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.ExchangeRequests.Queries.GetExchangeRequestById
{
    public class GetExchangeRequestByIdQueryHandler : IRequestHandler<GetExchangeRequestByIdQuery, ExchangeRequestDto>
    {
        private readonly IExchangeRequestRepository _repository;
        private readonly IMapper _mapper;

        public GetExchangeRequestByIdQueryHandler(IExchangeRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExchangeRequestDto> Handle(GetExchangeRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
                return null!;

            return _mapper.Map<ExchangeRequestDto>(entity);
        }
    }
}
