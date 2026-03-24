using Microsoft.EntityFrameworkCore;
using BookSwap.Aggregator.Models;

namespace BookSwap.Aggregator.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
    }
}