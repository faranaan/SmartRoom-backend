using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Models;

namespace SmartRoom.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set;}
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingLog> BookingLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(u => u.Role). HasConversion<string>();
            modelBuilder.Entity<Room>().Property(r => r.Type).HasConversion<string>();
            modelBuilder.Entity<Room>().Property(r => r.Building).HasConversion<string>();
            modelBuilder.Entity<Booking>().Property(b => b.Status).HasConversion<string>();
            modelBuilder.Entity<BookingLog>().Property(bl => bl.Action).HasConversion<string>();
        }
    }
}