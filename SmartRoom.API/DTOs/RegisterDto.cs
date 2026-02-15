using System.ComponentModel.DataAnnotations;
using SmartRoom.API.Models;

namespace SmartRoom.API.DTOs
{
    /// <summary>
    /// Data transfer object for user registration.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Username unique to use for login.
        /// </summary>
        /// <example>faran_123</example>
        [Required]
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Password for user account.
        /// </summary>
        /// <remarks> Password must be at least 8 characters. </remarks>
        /// <example>P@ssw0rd!</example>
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Role of the user in the system.
        /// </summary>
        /// <remarks> Default role is Mahasiswa.</remarks>
        /// <example>Mahasiswa</example>
        public UserRole Role { get; set; } = UserRole.Mahasiswa;

        /// <summary>
        /// Secret key required for Administrator registration
        /// </summary>
        /// <remarks>
        /// This field is only validated if the Role is set to 'Admin
        /// it is not stored in the database.
        /// </remarks>
        /// <example>SmartRoom2026</example>
        public string? AdminSecretKey { get; set; }
    }
}