namespace SmartRoom.API.Models
{
    public class Campus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AdminRegistrationToken { get; set; } = string.Empty;
        public string MemberRegistrationToken { get; set; } = string.Empty;
        public ICollection<User>? Users { get; set; }
        public ICollection<Room>? Rooms { get; set; }
        public ICollection<Booking>? Bookings {get; set; }
        public ICollection<Building>? Buildings { get; set; }
        public ICollection<RoomType>? RoomTypes { get; set; }
    }
}