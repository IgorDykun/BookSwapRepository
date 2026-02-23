using MediatR;

namespace BookSwap.Application.Transactions.Commands.DeleteTransaction
{
    public class DeleteTransactionCommand : IRequest<Unit>
    {
        public string Id { get; set; } = string.Empty;
    }
}
