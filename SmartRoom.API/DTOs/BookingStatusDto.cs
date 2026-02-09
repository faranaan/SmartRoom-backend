using System.ComponentModel.DataAnnotations;
using SmartRoom.API.Models;

namespace SmartRoom.API.DTOs
{
    public class BookingStatusDto
    {
        [Required]
        public BookingStatus Status { get; set; }
    }
}