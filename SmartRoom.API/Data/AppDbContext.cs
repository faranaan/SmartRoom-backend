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
        public DbSet<Building> Buildings { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(u => u.Role). HasConversion<string>();
            modelBuilder.Entity<Booking>().Property(b => b.Status).HasConversion<string>();
            modelBuilder.Entity<BookingLog>().Property(bl => bl.Action).HasConversion<string>();
            modelBuilder.Entity<Booking>().Property(b => b.StartTime).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<Booking>().Property(b => b.EndTime).HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Building>()
                .HasOne(b => b.Campus)
                .WithMany(c => c.Buildings)
                .HasForeignKey(b => b.CampusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomType>()
                .HasOne(rt => rt.Campus)
                .WithMany(c => c.RoomTypes)
                .HasForeignKey(rt => rt.CampusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(b => b.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Building)
                .WithMany(b => b.Rooms)
                .HasForeignKey(r => r.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Campus)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CampusId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<User>()
                .HasOne(u => u.Campus)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CampusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Campus>().HasData(
                new Campus
                {
                    Id = 1,
                    Name = "Politeknik Elektronika Negeri Surabaya",
                    AdminRegistrationToken = "ADM-123",
                    MemberRegistrationToken = "MBR-123"
                }
            );

            modelBuilder.Entity<Building>().HasData(
                new Building { Id = 1, CampusId = 1, Name = "Gedung D4"},
                new Building { Id = 2, CampusId = 1, Name = "Gedung D3"},
                new Building { Id = 3, CampusId = 1, Name = "SAW"},
                new Building { Id = 4, CampusId = 1, Name = "Pascasarjana"}
            );

            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { Id = 1, CampusId = 1, Name = "Classroom" },
                new RoomType { Id = 2, CampusId = 1, Name = "Laboratory" },
                new RoomType { Id = 3, CampusId = 1, Name = "Meeting Room" },
                new RoomType { Id = 4, CampusId = 1, Name = "Auditorium" }
            );

            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, CampusId = 1, RoomName = "C303", RoomTypeId = 2, BuildingId = 1, Capacity = 60, IsAvailable = true },
                new Room { Id = 2, CampusId = 1, RoomName = "HH106", RoomTypeId = 1, BuildingId = 2, Capacity = 30, IsAvailable = true },
                new Room { Id = 3, CampusId = 1, RoomName = "SAW 0708", RoomTypeId = 3, BuildingId = 3, Capacity = 120, IsAvailable = true },
                new Room { Id = 4, CampusId = 1, RoomName = "Auditorium 501", RoomTypeId = 4, BuildingId = 4, Capacity = 300, IsAvailable = true }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "superadmin", Password = BCrypt.Net.BCrypt.HashPassword("#superadmin1234"), Role = UserRole.SuperAdmin, CampusId = null, Email = "superadmin@smartroom.com"}
            );
        }
    }
}