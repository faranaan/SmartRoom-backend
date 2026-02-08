using System.ComponentModel.DataAnnotations;
using SmartRoom.API.Models;

namespace SmartRoom.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Mahasiswa;
    }
}