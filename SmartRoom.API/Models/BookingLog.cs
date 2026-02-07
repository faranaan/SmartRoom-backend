namespace SmartRoom.API.Models
{
    public enum ActionType
    {
        Created,
        Approved,
        Rejected
    }

    public class BookingLog
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public ActionType Action { get; set; } = ActionType.Created;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}