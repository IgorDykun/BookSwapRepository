using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSwap.Aggregator.Models
{
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Language { get; set; } = "uk";
        public string FavoriteCity { get; set; } = string.Empty;
    }
}