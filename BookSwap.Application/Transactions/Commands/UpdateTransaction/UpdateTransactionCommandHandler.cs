using AutoMapper;
using BookSwap.Application.Transactions.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.Transactions.Commands.UpdateTransaction
{
    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, TransactionDto>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;

        public UpdateTransactionCommandHandler(ITransactionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TransactionDto> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _repository.GetByIdAsync(request.Id);
            if (transaction == null)
                throw new Exception("Transaction not found");

            if (request.User1Confirmed.HasValue)
                transaction.User1Confirmed = request.User1Confirmed.Value;

            if (request.User2Confirmed.HasValue)
                transaction.User2Confirmed = request.User2Confirmed.Value;

            if (transaction.User1Confirmed && transaction.User2Confirmed)
                transaction.CompletedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(transaction.Id, transaction);

            return _mapper.Map<TransactionDto>(transaction);
        }
    }
}
