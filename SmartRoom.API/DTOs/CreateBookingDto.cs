using System.ComponentModel.DataAnnotations;

namespace SmartRoom.API.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}