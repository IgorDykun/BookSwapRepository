using MediatR;
using BookSwap.Application.Transactions.Dtos;

namespace BookSwap.Application.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionCommand : IRequest<TransactionDto>
    {
        public string ExchangeRequestId { get; set; } = string.Empty;
    }
}
