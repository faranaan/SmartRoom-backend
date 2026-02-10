using System.ComponentModel.DataAnnotations;

namespace SmartRoom.API.DTOs
{
    /// <summary>
    /// Data transfer for object for creating a new booking room.
    /// </summary>
    public class CreateBookingDto
    {
        /// <summary>
        /// ID unique of the room to be booked.
        /// </summary>
        /// <example>1</example>
        [Required]
        public int RoomId { get; set; }

        /// <summary>
        /// Start time of the booking.
        /// </summary>
        /// <remarks> Format: YYYY-MM-DDTHH:MM:SSZ using UTC timezone</remarks>
        /// <example>2024-07-01T10:00:00Z</example>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time of the booking.
        /// </summary>
        /// <remarks> Format: YYYY-MM-DDTHH:MM:SSZ using UTC timezone. Must be higher than Start Time</remarks>
        /// <example>2024-07-01T12:00:00Z</example>
        [Required]
        public DateTime EndTime { get; set; }

        /// <summary>
        ///  Description or purpose of the booking.
        /// </summary>
        /// <example>Meeting with team members</example>
        public string Description { get; set; } = string.Empty;
    }
}