using MediatR;
using BookSwap.Application.Transactions.Dtos;
using System.Collections.Generic;

namespace BookSwap.Application.Transactions.Queries.GetTransactionsList
{
    public class GetTransactionsListQuery : IRequest<List<TransactionDto>>
    {
    }
}
