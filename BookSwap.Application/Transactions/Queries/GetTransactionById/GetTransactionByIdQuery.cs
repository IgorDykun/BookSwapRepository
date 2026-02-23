using MediatR;
using BookSwap.Application.Transactions.Dtos;

namespace BookSwap.Application.Transactions.Queries.GetTransactionById
{
    public class GetTransactionByIdQuery : IRequest<TransactionDto>
    {
        public string Id { get; set; } = string.Empty;
    }
}
