using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.ExchangeRequests.Commands.ConfirmExchangeRequest
{
    public class ConfirmExchangeRequestCommandHandler : IRequestHandler<ConfirmExchangeRequestCommand, bool>
    {
        private readonly IExchangeRequestRepository _repository;

        public ConfirmExchangeRequestCommandHandler(IExchangeRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(ConfirmExchangeRequestCommand request, CancellationToken cancellationToken)
        {
            var exchangeRequest = await _repository.GetByIdAsync(request.Id);
            if (exchangeRequest == null)
                return false; 

            if (exchangeRequest.FromUser.Id == request.UserId)
                exchangeRequest.FromUserAccepted = true;
            else if (exchangeRequest.ToUser.Id == request.UserId)
                exchangeRequest.ToUserAccepted = true;
            else
                return false; 

            if (exchangeRequest.FromUserAccepted && exchangeRequest.ToUserAccepted)
                exchangeRequest.Status = "Confirmed";

            await _repository.UpdateAsync(exchangeRequest.Id, exchangeRequest);

            return true;
        }
    }
}
