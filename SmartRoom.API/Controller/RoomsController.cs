using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Data;
using SmartRoom.API.Models;
using System.Security.Claims;

namespace SmartRoom.API.Controller
{
    /// <summary>
    /// Controller for managing rooms in the SmartRoom system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieve all rooms available in the system.
        /// </summary>
        /// <returns>List of all rooms in the system</returns>
        /// <response code="200">Returns the list of rooms</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var campusIdStr = User.FindFirst("CampusId")?.Value;

            IQueryable<Room> query = _context.Rooms
                .Include(r => r.Building)
                .Include(r => r.RoomType);

            if (userRole != UserRole.SuperAdmin.ToString())
            {
                if (string.IsNullOrEmpty(campusIdStr))
                {
                    return Unauthorized("Campus ID is missing in your token.");
                }

                if (int.TryParse(campusIdStr, out int campusId))
                {
                    query = query.Where(r => r.CampusId == campusId);
                }
                else
                {
                    return BadRequest("Invalid Campus ID format in token.");
                }
            }
            var rooms = await query.ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var room in rooms)
            {
                bool isBooked = _context.Bookings.Any(b =>
                    b.RoomId == room.Id &&
                    b.Status == BookingStatus.Approved &&
                    now >= b.StartTime &&
                    now <= b.EndTime);

                room.IsAvailable = !isBooked;
            }

            return Ok(rooms);
            
        }

        /// <summary>
        /// Retrieve a specific room by its ID.
        /// </summary>
        /// <param name="id">ID unique of the room to retrieve</param>
        /// <returns>Object of the requested room</returns>
        /// <response code="200">Returns the requested room</response>
        /// <response code="404">Room not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Building)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return NotFound("Room not found.");

            var now = DateTime.UtcNow;

            bool isBooked = await _context.Bookings.AnyAsync(b =>
                b.RoomId == room.Id &&
                b.Status == BookingStatus.Approved &&
                now >= b.StartTime &&
                now <= b.EndTime);

            room.IsAvailable = !isBooked;

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var campusIdStr = User.FindFirst("CampusId")?.Value;

            if (userRole != UserRole.SuperAdmin.ToString())
            {
                if (room.CampusId.ToString() != campusIdStr) return Forbid("You are not authorized to view this room.");
            }

            return Ok(room);
        }

        /// <summary>
        /// Create a new room in the system.
        /// </summary>
        /// <remarks> Only accessible by Admin users. </remarks>
        /// <param name="room">Data of the room to be created</param>
        /// <response code="201">Room created successfully</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            var campusIdStr = User.FindFirst("CampusId")?.Value;
            if (string.IsNullOrEmpty(campusIdStr)) return Unauthorized("Admin must belong to a campus.");
            if (string.IsNullOrWhiteSpace(room.RoomName)) return BadRequest("Room name cannot be empty.");
            if (!int.TryParse(campusIdStr, out int campusId)) return BadRequest("Invalid Campus ID in token.");

            room.CampusId = campusId;
            room.Building = null;
            room.RoomType = null;
            room.Campus = null;

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }

        /// <summary>
        /// Update an existing room's details.
        /// </summary>
        /// <param name="id">Id of the room to update</param>
        /// <param name="room">Updated room data</param>
        /// <response code="204">Room updated successfully</response>
        /// <response code="400">Room ID mismatch or invalid data</response>
        /// <response code="404">Room not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutRoom(int id, Room room)
        {
            if (id != room.Id) return BadRequest("Room ID mismatch");

            var existingRoom = await _context.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (existingRoom == null) return NotFound("Room not found.");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var campusIdStr = User.FindFirst("CampusId")?.Value;

            if (userRole != UserRole.SuperAdmin.ToString())
            {
                if (existingRoom.CampusId.ToString() != campusIdStr)
                {
                    return Forbid("You cannot update a room from another campus.");
                }

                room.CampusId = existingRoom.CampusId;
            }
            
            room.Building = null;
            room.RoomType = null;
            room.Campus = null;

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            } 
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound("Room not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a room from the system.
        /// </summary>
        /// <param name="id">ID of the room to delete</param>
        /// <response code="204">Room deleted successfully</response>
        /// <response code="404">Room not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound("Room not found.");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var campusIdStr = User.FindFirst("CampusId")?.Value;

            if (userRole != UserRole.SuperAdmin.ToString())
            {
                if (room.CampusId.ToString() != campusIdStr)
                {
                    return Forbid("You cannot delete a room from another campus.");
                }
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Function to check if a room exists by its ID in database.
        /// </summary>
        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
