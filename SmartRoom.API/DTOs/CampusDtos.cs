using System.ComponentModel.DataAnnotations;

namespace SmartRoom.API.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new Campus entity.
    /// </summary>
    /// <remarks>
    /// This DTO is used by SuperAdmins to initialize a new tenant (Campus) in the system.
    /// </remarks>
    public class CreateCampusDto
    {
        /// <summary>
        /// The official name of the Campus.
        /// </summary>
        /// <example>Universitas Indonesia</example>
        [Required(ErrorMessage = "Campus name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data Transfer Object representing the response after a Campus is created or fetched.
    /// </summary>
    /// <remarks>
    /// Contains generated registration tokens that must be distributed to designated Admins or Students.
    /// </remarks>
    public class CampusResponseDto
    {
        /// <summary>
        /// Unique identifier for the Campus.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The name of the registered Campus.
        /// </summary>
        /// <example>Universitas Indonesia</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Secret token used to register an Administrator for this specific campus.
        /// </summary>
        /// <remarks>
        /// This token should be kept secure and only given to the authorized Campus Admin.
        /// </remarks>
        /// <example>ADM-UI-2026-XYZ</example>
        public string AdminRegistrationToken { get; set; } = string.Empty;

        /// <summary>
        /// Token used by students to register and bind themselves to this campus.
        /// </summary>
        /// <remarks>
        /// Can be shared more widely (e.g., via campus email or portal) for student onboarding.
        /// </remarks>
        /// <example>STU-UI-2026-ABC</example>
        public string MemberRegistrationToken { get; set; } = string.Empty;
    }
}