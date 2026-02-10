using System.ComponentModel.DataAnnotations;

namespace SmartRoom.API.DTOs
{
    /// <summary>
    /// Data transfer object for user login.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Username of the user registered in the system.
        /// </summary>
        /// <example>faran_123</example>
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password associated with the username.
        /// </summary>
        /// <example>P@ssw0rd!</example>
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}