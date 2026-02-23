using MongoDB.Bson.Serialization.Attributes;

namespace BookSwap.Domain.Entities
{
    public class UserDocument : BaseEntity
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("joinedAt")]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

}
