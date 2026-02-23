using MediatR;
using BookSwap.Application.Transactions.Dtos;

namespace BookSwap.Application.Transactions.Commands.UpdateTransaction
{
    public class UpdateTransactionCommand : IRequest<TransactionDto>
    {
        public string Id { get; set; } = string.Empty;
        public bool? User1Confirmed { get; set; }
        public bool? User2Confirmed { get; set; }
    }
}
