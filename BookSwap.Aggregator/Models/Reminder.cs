using System.ComponentModel.DataAnnotations;

namespace BookSwap.Aggregator.Models
{
    public class Reminder
    {
        [Key]
        public int Id { get; set; }
        public long UserId { get; set; } 
        public string Text { get; set; } = string.Empty; 
        public DateTime NotifyAt { get; set; } 
        public bool IsSent { get; set; } = false; 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}