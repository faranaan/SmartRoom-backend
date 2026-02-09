using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SmartRoom.API.Models
{
    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }

    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room? Room { get; set;}

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string Description { get; set;} = string.Empty;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [JsonIgnore]
        public ICollection<BookingLog>? Logs { get; set; }
        
    }
}