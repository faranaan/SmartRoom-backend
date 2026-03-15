using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Data;
using SmartRoom.API.Models;

namespace SmartRoom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomTypesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomType>>> GetRoomTypes()
        {
            var campusIdStr = User.FindFirst("CampusId")?.Value;
            if (string.IsNullOrEmpty(campusIdStr)) return Unauthorized("Campus ID missing.");

            int campusId = int.Parse(campusIdStr);
            var roomTypes = await _context.RoomTypes
                .Where(rt => rt.CampusId == campusId)
                .ToListAsync();

            return Ok(roomTypes);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomType>> CreateRoomType(RoomType roomType)
        {
            var campusIdStr = User.FindFirst("CampusId")?.Value;
            if (string.IsNullOrEmpty(campusIdStr)) return Unauthorized("Campus ID missing.");

            roomType.CampusId = int.Parse(campusIdStr);

            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoomTypes), new { id = roomType.Id }, roomType);
        }
    }
}