using AutoMapper;
using BookSwap.Application.Transactions.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;

        public CreateTransactionCommandHandler(ITransactionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = new TransactionDocument
            {
                ExchangeRequestId = request.ExchangeRequestId,
                User1Confirmed = false,
                User2Confirmed = false
            };

            await _repository.CreateAsync(transaction);

            return _mapper.Map<TransactionDto>(transaction);
        }
    }
}
