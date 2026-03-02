using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Models;

namespace SmartRoom.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Campus> Campuses { get; set; }
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

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Campus)
                .WithMany(c => c.Rooms)
                .HasForeignKey(r => r.CampusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Campus)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CampusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Campus)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CampusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Campus>().HasData(
                new Campus
                {
                    Id = 1,
                    Name = "Kampys Utama",
                    AdminRegistrationToken = "ADM-123",
                    StudentRegistrationToken = "MHS-123"
                }
            );

            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, CampusId = 1, RoomName = "Classroom 101", Capacity = 30, Type = RoomType.Classroom, Building = BuildingType.TowerA, IsAvailable = true },
                new Room { Id = 2, CampusId = 1, RoomName = "Laboratory 202", Capacity = 20, Type = RoomType.Laboratory, Building = BuildingType.TowerB, IsAvailable = true },
                new Room { Id = 3, CampusId = 1, RoomName = "Meeting Room 303", Capacity = 15, Type = RoomType.MeetingRoom, Building = BuildingType.TowerC, IsAvailable = true },
                new Room { Id = 4, CampusId = 1, RoomName = "Auditorium 404", Capacity = 100, Type = RoomType.Auditorium, Building = BuildingType.TowerA, IsAvailable = true }  
            );
        }
    }
}