using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Data;
using SmartRoom.API.Models;
using SmartRoom.API.DTOs;
using System.Security.Claims;

namespace SmartRoom.API.Controller
{
    /// <summary>
    /// Controller for managing bookings in the SmartRoom system. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieve all bookings in the system.
        /// </summary>
        /// <remarks> Only accessible by Admin users. </remarks>
        /// <returns>List of all bookings in the system</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b=> b.User)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieve bookings made by the authenticated user.
        /// </summary>
        /// <returns>List of bookings made by the authenticated user</returns>
        [HttpGet("my-bookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetMyBookings()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            return await _context.Bookings.Where(b => b.UserId == userId).Include(b => b.Room).OrderByDescending(b => b.StartTime).ToListAsync();
        }

        /// <summary>
        /// Retrieves a list of bookings for a specific room to be displayed in the room detail schedule.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room.</param>
        /// <returns>A list of objects containing booking time, status, and the borrower's username.</returns>
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetBookingsByRoom(int roomId)
        {
            var bookings = await _context.Bookings.Include(b => b.User).Where(b => b.RoomId == roomId).OrderByDescending(b => b.StartTime).Select(b => new
            {
                b.Id,
                b.StartTime,
                b.EndTime,
                b.Status,
                Username = b.User != null ? b.User.Username : "Unknown User",
                Purpose = b.Description
            }).ToListAsync();

            return Ok(bookings);
        }

        /// <summary>
        /// Retrieves the audit trail logs for a specific room, showing all actions (Create, Approve, Reject, Cancel).
        /// </summary>
        /// <param name="roomId">The unique identifier of the room.</param>
        /// <returns>A list of log entries detailing actions performed, the actor, and associated booking info.</returns>
        [HttpGet("room/{roomId}/logs")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoomLogs(int roomId)
        {
            var logs = await _context.BookingLogs.Include(l => l.Booking).ThenInclude(b => b.User).Where(l => l.Booking.RoomId == roomId).OrderByDescending(l => l.TimeStamp).Select(l => new
            {
                l.Id,
                l.Action,
                l.PerformedBy,
                l.TimeStamp,
                l.Notes,
                Peminjam = (l.Booking != null && l.Booking.User != null) ? l.Booking.User.Username : "System"
            }).ToListAsync();

            return Ok(logs);
        }

        /// <summary>
        /// Create a new booking in the system.
        /// </summary>
        /// <remarks> Booking times are stored in UTC format, EndTime must be after startTime, system checks for conflicts. </remarks>
        /// <param name="request">Data of the booking to be created</param>
        /// <response code="201">Booking created successfully</response>
        /// <response code="400">Invalid input data or room is already booked for the selected time range</response>
        /// <response code="401">Unauthorized user</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Booking>> CreateBooking(CreateBookingDto request)
        {
            // convert time to UTC
            var startTimeUtc = request.StartTime.ToUniversalTime();
            var endTImeUtc = request.EndTime.ToUniversalTime();

            // date validation
            if (startTimeUtc >= endTImeUtc)
            {
                return BadRequest("End date must be after start date.");
            }

            if(startTimeUtc < DateTime.UtcNow) return BadRequest("Cannot booking in the past.");

            // availability validation
            bool isConflict = await _context.Bookings.AnyAsync(b => b.RoomId == request.RoomId && b.Status != BookingStatus.Rejected && b.Status != BookingStatus.Cancelled && (startTimeUtc < b.EndTime && endTImeUtc > b.StartTime));

            if (isConflict)
            {
                return BadRequest("The room is already booked for the selected time range.");
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
            if(string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);
            var username = User.Identity?.Name ?? "Unknown User";

            // save booking
            var booking = new Booking
            {
                RoomId = request.RoomId,
                UserId = userId,
                StartTime = startTimeUtc,
                EndTime = endTImeUtc,
                Description = request.Description,
                Status = BookingStatus.Pending
            };

            _context.Bookings.Add(booking);

            var log = new BookingLog
            {
                Booking= booking,
                Action = ActionType.Created,
                PerformedBy = username,
                TimeStamp = DateTime.UtcNow,
                Notes = "Booking created at the first time"
            };
            _context.BookingLogs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, new
            {
                booking.Id, booking.Status, Message = "Booking created successfully"
            });
        }

        /// <summary>
        /// Update the status of a booking.
        /// </summary>
        /// <param name="id">Id of the booking to update.</param>
        /// <param name="request">new status of the booking (approved, rejected, pending, cancelled)</param>
        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] BookingStatusDto request)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var username = User.Identity?.Name ?? "Unknown";

            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            if (request.Status == BookingStatus.Approved || request.Status == BookingStatus.Rejected)
            {
                if (userRole != "Admin") return Forbid();
            } else if (request.Status == BookingStatus.Cancelled)
            {
                if (userRole != "Admin" && booking.UserId != userId) return Forbid();
            }

            var oldStatus = booking.Status;
            booking.Status =request.Status;

            var logAction = request.Status switch
            {
                BookingStatus.Approved => ActionType.Approved,
                BookingStatus.Rejected => ActionType.Rejected,
                BookingStatus.Cancelled => ActionType.Cancelled,
                _ => ActionType.Created
            };

            var log = new BookingLog
            {
                BookingId = booking.Id,
                Action = logAction,
                PerformedBy = username,
                TimeStamp = DateTime.UtcNow,
                Notes = request.Notes ?? $"Status changed from {oldStatus} to {booking.Status}"
            };
            _context.BookingLogs.Add(log);

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Booking status updated", status = booking.Status });
        }
    }
}