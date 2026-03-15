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
    public class BuildingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BuildingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuildings()
        {
            var campusIdStr = User.FindFirst("CampusId")?.Value;
            if (string.IsNullOrEmpty(campusIdStr)) return Unauthorized("Campus ID missing.");

            int campusId = int.Parse(campusIdStr);
            var buildings = await _context.Buildings
                .Where(b => b.CampusId == campusId)
                .ToListAsync();

            return Ok(buildings);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Building>> CreateBuilding(Building building)
        {
            var campusIdStr = User.FindFirst("CampusId")?.Value;
            if(string.IsNullOrEmpty(campusIdStr)) return Unauthorized("Campus ID missing");

            building.CampusId = int.Parse(campusIdStr);

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBuildings), new { id = building.Id }, building);
        }
    }
}