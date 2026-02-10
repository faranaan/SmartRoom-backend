using System.ComponentModel.DataAnnotations;
using SmartRoom.API.Models;

namespace SmartRoom.API.DTOs
{
    /// <summary>
    /// Data transfer object for updating booking status.
    /// </summary>
    public class BookingStatusDto
    {
        /// <summary>
        /// Status of the booking.
        /// </summary>
        /// <remarks> Possible values: Pending, Approved, Rejected </remarks>
        [Required]
        public BookingStatus Status { get; set; }
    }
}