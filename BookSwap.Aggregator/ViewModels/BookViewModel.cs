namespace BookSwap.Aggregator.ViewModels
{
    public class BookViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public UserSummaryViewModel Owner { get; set; } = new UserSummaryViewModel();
    }

    public class UserSummaryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
