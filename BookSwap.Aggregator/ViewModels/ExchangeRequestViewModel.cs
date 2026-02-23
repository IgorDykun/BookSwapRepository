namespace BookSwap.Aggregator.ViewModels
{
    public class ExchangeRequestViewModel
    {
        public string Id { get; set; } = string.Empty;
        public UserSummaryViewModel FromUser { get; set; } = new UserSummaryViewModel();
        public UserSummaryViewModel ToUser { get; set; } = new UserSummaryViewModel();
        public BookSummaryViewModel BookOffered { get; set; } = new BookSummaryViewModel();
        public BookSummaryViewModel BookRequested { get; set; } = new BookSummaryViewModel();
        public string Status { get; set; } = "Pending";
        public DateTime UpdatedAt { get; set; }
    }

    public class BookSummaryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
