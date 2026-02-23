using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.ExchangeRequests.Commands.DeleteExchangeRequest
{
    public class DeleteExchangeRequestCommandHandler : IRequestHandler<DeleteExchangeRequestCommand, Unit>
    {
        private readonly IExchangeRequestRepository _repository;
        public DeleteExchangeRequestCommandHandler(IExchangeRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteExchangeRequestCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }
}
