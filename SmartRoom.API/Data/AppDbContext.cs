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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(u => u.Role). HasConversion<string>();
            modelBuilder.Entity<Room>().Property(r => r.Type).HasConversion<string>();
            modelBuilder.Entity<Room>().Property(r => r.Building).HasConversion<string>();
            modelBuilder.Entity<Booking>().Property(b => b.Status).HasConversion<string>();
            modelBuilder.Entity<BookingLog>().Property(bl => bl.Action).HasConversion<string>();

            modelBuilder.Entity<Booking>().Property(b => b.StartTime).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<Booking>().Property(b => b.EndTime).HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, RoomName = "Classroom 101", Capacity = 30, Type = RoomType.Classroom, Building = BuildingType.TowerA, IsAvailable = true },
                new Room { Id = 2, RoomName = "Laboratory 202", Capacity = 20, Type = RoomType.Laboratory, Building = BuildingType.TowerB, IsAvailable = true },
                new Room { Id = 3, RoomName = "Meeting Room 303", Capacity = 15, Type = RoomType.MeetingRoom, Building = BuildingType.TowerC, IsAvailable = true },
                new Room { Id = 4, RoomName = "Auditorium 404", Capacity = 100, Type = RoomType.Auditorium, Building = BuildingType.TowerA, IsAvailable = true }    
            );
        }
    }
}