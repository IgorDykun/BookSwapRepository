using AutoMapper;
using BookSwap.Application.ExchangeRequests.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.ExchangeRequests.Commands.UpdateExchangeRequest
{
    public class UpdateExchangeRequestCommandHandler : IRequestHandler<UpdateExchangeRequestCommand, ExchangeRequestDto>
    {
        private readonly IExchangeRequestRepository _repository; 
        private readonly IMapper _mapper;

        public UpdateExchangeRequestCommandHandler(IExchangeRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExchangeRequestDto> Handle(UpdateExchangeRequestCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(request.Id);
            if (existing == null)
                throw new Exception("Exchange request not found");

            existing.Status = request.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing.Id, existing);

            return _mapper.Map<ExchangeRequestDto>(existing);
        }
    }
}
