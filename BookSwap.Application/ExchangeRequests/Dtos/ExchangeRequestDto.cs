namespace BookSwap.Application.ExchangeRequests.Dtos
{
    public class ExchangeRequestDto
    {
        public string Id { get; set; } = string.Empty;
        public UserSummaryDto FromUser { get; set; } = new UserSummaryDto();
        public UserSummaryDto ToUser { get; set; } = new UserSummaryDto();
        public BookSummaryDto BookOffered { get; set; } = new BookSummaryDto();
        public BookSummaryDto BookRequested { get; set; } = new BookSummaryDto();
        public string Status { get; set; } = "Pending";
        public DateTime UpdatedAt { get; set; }
    }

    public class UserSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class BookSummaryDto
    {
        public string Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
