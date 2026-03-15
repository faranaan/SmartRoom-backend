using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Data;
using SmartRoom.API.DTOs;
using SmartRoom.API.Models;
using System.Security.Claims;

namespace SmartRoom.API.Controllers
{
    /// <summary>
    /// Controller for managing campus entities and their respective registration tokens (Multi-tenant management).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CampusesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CampusesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all campuses including their registration tokens.
        /// </summary>
        /// <remarks>Only accessible by users with the 'SuperAdmin' role.</remarks>
        /// <returns>A list of all campuses registered in the system.</returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<IEnumerable<CampusResponseDto>>> GetCampuses()
        {
            var campuses = await _context.Campuses.Select(c => new CampusResponseDto {
                Id = c.Id,
                Name = c.Name,
                AdminRegistrationToken = c.AdminRegistrationToken,
                MemberRegistrationToken = c.MemberRegistrationToken
            }).ToListAsync();

            return Ok(campuses);
        }

        /// <summary>
        /// Creates a new campus and generates initial registration tokens.
        /// </summary>
        /// <param name="request">The data required to create a campus (e.g., Name).</param>
        /// <returns>The newly created campus object with generated tokens.</returns>
        /// <response code="200">Returns the created campus details.</response>
        /// <response code="401">Unauthorized if the user is not a SuperAdmin.</response>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<CampusResponseDto>> CreateCampus(CreateCampusDto request)
        {
            var campus = new Campus
            {
                Name = request.Name,
                AdminRegistrationToken = "ADM-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                MemberRegistrationToken = "MBR-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()
            };

            _context.Campuses.Add(campus);
            await _context.SaveChangesAsync();

            return Ok(new CampusResponseDto { Id = campus.Id, Name = campus.Name, AdminRegistrationToken = campus.AdminRegistrationToken, MemberRegistrationToken = campus.MemberRegistrationToken });
        }

        /// <summary>
        /// Regenerates a new Admin Registration Token for a specific campus.
        /// </summary>
        /// <param name="id">The unique identifier of the campus.</param>
        /// <remarks>Use this if a token is compromised or needs to be refreshed by a SuperAdmin.</remarks>
        [HttpPost("{id}/generate-admin-token")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GenerateAdminToken(int id)
        {
            var campus = await _context.Campuses.FindAsync(id);
            if (campus == null ) return NotFound("Campus not found");

            campus.AdminRegistrationToken = "ADM-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            await _context.SaveChangesAsync();

            return Ok(new { Message = "New Admin Token Generated", Token = campus.AdminRegistrationToken });
        }

        /// <summary>
        /// Regenerates a new Member/Student Token for the campus associated with the logged-in Admin.
        /// </summary>
        /// <remarks>
        /// This endpoint automatically identifies the campus via the 'CampusId' claim in the authenticated user's JWT.
        /// </remarks>
        /// <returns>A new member token to be shared with students or staff.</returns>
        [HttpPost("my-campus/generate-member-token")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateMemberToken()
        {
            var campusIdStr = User.FindFirst("CampusId")?.Value;
            if (string.IsNullOrEmpty(campusIdStr)) return Unauthorized("invalid Token");

            var campusId = int.Parse(campusIdStr);
            var campus = await _context.Campuses.FindAsync(campusId);
            if (campus == null) return NotFound("Campus not found");

            campus.MemberRegistrationToken = "MBR-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            await _context.SaveChangesAsync();

            return Ok(new { Message = "New Student Token Generated", Token = campus.MemberRegistrationToken});
        }
    }
}