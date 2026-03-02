namespace SmartRoom.API.Models
{
    public enum UserRole
    {
        SuperAdmin,
        Admin,
        Mahasiswa,
        Dosen
    }

    public class User{
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Mahasiswa;
        public string? Email { get; set; }
        public int? CampusId { get; set; }
        public Campus? Campus { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}