using MongoDB.Bson.Serialization.Attributes;

namespace BookSwap.Domain.Entities
{
    public class BookDocument : BaseEntity
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("author")]
        public string Author { get; set; } = string.Empty;

        [BsonElement("owner")]
        public UserSummaryDocument Owner { get; set; } = new UserSummaryDocument();
    }

    public class UserSummaryDocument
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
    }
}
