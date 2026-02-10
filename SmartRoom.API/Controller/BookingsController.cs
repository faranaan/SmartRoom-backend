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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            return await _context.Bookings.Where(b => b.UserId == userId).Include(b => b.Room).OrderByDescending(b => b.StartTime).ToListAsync();
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

        /// <summary>
        /// Update the status of a booking.
        /// </summary>
        /// <param name="id">Id of the booking to update.</param>
        /// <param name="request">new status of the booking (approved, rejected, pending, cancelled)</param>
        /// <response code="200">Booking status updated successfully</response>
        /// <response code="404">Booking not found</response>
        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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