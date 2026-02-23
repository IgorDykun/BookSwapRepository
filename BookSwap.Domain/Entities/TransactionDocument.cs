using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BookSwap.Domain.Entities
{
    public class TransactionDocument : BaseEntity
    {
        [BsonElement("exchangeRequestId")]
        public string ExchangeRequestId { get; set; } = string.Empty;

        [BsonElement("user1Confirmed")]
        public bool User1Confirmed { get; set; }

        [BsonElement("user2Confirmed")]
        public bool User2Confirmed { get; set; }

        [BsonElement("completedAt")]
        public DateTime? CompletedAt { get; set; }
    }

}
