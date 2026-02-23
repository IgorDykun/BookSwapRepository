using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BookSwap.Domain.Entities
{
    public class ExchangeRequestDocument : BaseEntity
    {
        [BsonElement("fromUser")]
        public UserSummaryDocument FromUser { get; set; } = new UserSummaryDocument();

        [BsonElement("toUser")]
        public UserSummaryDocument ToUser { get; set; } = new UserSummaryDocument();

        [BsonElement("bookOffered")]
        public BookSummary BookOffered { get; set; } = new BookSummary();

        [BsonElement("bookRequested")]
        public BookSummary BookRequested { get; set; } = new BookSummary();

        [BsonElement("fromUserAccepted")]
        public bool FromUserAccepted { get; set; }

        [BsonElement("toUserAccepted")]
        public bool ToUserAccepted { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pending";

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }


    public class BookSummary
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
    }

}
