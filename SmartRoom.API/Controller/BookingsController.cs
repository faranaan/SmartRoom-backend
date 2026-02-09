using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Data;
using SmartRoom.API.Models;
using SmartRoom.API.DTOs;
using System.Security.Claims;

namespace SmartRoom.API.Controller
{
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b=> b.User)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        [HttpGet("my-bookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetMyBookings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            return await _context.Bookings.Where(b => b.UserId == userId).Include(b => b.Room).OrderByDescending(b => b.StartTime).ToListAsync();
        }

        [HttpPost]
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

            // availability validation
            bool isConflict = await _context.Bookings.AnyAsync(b => b.RoomId == request.RoomId && b.Status != BookingStatus.Rejected && (startTimeUtc < b.EndTime && endTImeUtc > b.StartTime));

            if (isConflict)
            {
                return BadRequest("The room is already booked for the selected time range.");
            }

            // get user id from token
            var userIdClaim = User.FindFirst("id");
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return Unauthorized("User nof found.");
            }

            // save booking
            var booking = new Booking
            {
                RoomId = request.RoomId,
                UserId = user.Id,
                StartTime = startTimeUtc,
                EndTime = endTImeUtc,
                Description = request.Description,
                Status = BookingStatus.Pending
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
        }


        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, BookingStatusDto request)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            booking.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Booking status updated to {request.Status}", data = booking });
        }
    }
}