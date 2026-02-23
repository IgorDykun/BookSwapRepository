namespace BookSwap.Aggregator.ViewModels
{
    public class TransactionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string ExchangeRequestId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; 
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
