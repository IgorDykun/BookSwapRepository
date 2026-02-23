namespace BookSwap.Application.Transactions.Dtos
{
    public class TransactionDto
    {
        public string Id { get; set; } = string.Empty;
        public string ExchangeRequestId { get; set; } = string.Empty;
        public bool User1Confirmed { get; set; }
        public bool User2Confirmed { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
