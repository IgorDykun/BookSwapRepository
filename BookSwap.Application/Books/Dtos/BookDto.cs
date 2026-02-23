namespace BookSwap.Application.Books.Dtos
{
    public class BookDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public UserSummaryDto Owner { get; set; } = new UserSummaryDto();
    }

    public class UserSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
