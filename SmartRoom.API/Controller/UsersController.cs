using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Data;
using SmartRoom.API.DTOs;
using System.Security.Claims;

namespace SmartRoom.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var user = await _context.Users.Include(u => u.Campus).FirstOrDefaultAsync(u => u.Id == int.Parse(userIdStr));

            if (user == null) return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Role,
                user.Email,
                CampusName = user.Campus?.Name ?? "SuperAdmin (No Campus)"
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var user = await _context.Users.FindAsync(int.Parse(userIdStr));
            if (user == null) return NotFound("User not found");

            user.Email = request.Email;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Profile updated successfully", Email = user.Email});
        }
    }
}