using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Data;
using SmartRoom.API.DTOs;
using System.Security.Claims;

namespace SmartRoom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminDashboardDto>> GetAdminStats()
        {
            var today = DateTime.UtcNow.Date;

            var stats = new AdminDashboardDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalRooms = await _context.Rooms.CountAsync(),
                PendingBookings = await _context.Bookings.CountAsync(b => b.Status == Models.BookingStatus.Pending),
                ActiveBookingsToday = await _context.Bookings.CountAsync(b => 
                    b.Status == Models.BookingStatus.Approved && 
                    b.StartTime.Date == today)
            };

            return Ok(stats);
        }

        [HttpGet("student")]
        public async Task<ActionResult<StudentDashboardDto>> GetStudentStats()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            var stats = new StudentDashboardDto
            {
                MyActiveBookings = await _context.Bookings.CountAsync(b => 
                    b.UserId == userId && 
                    (b.Status == Models.BookingStatus.Pending || 
                    (b.Status == Models.BookingStatus.Approved && b.EndTime > DateTime.UtcNow))),
                
                MyTotalBookings = await _context.Bookings.CountAsync(b => b.UserId == userId),
                
                MyRejectedBookings = await _context.Bookings.CountAsync(b => b.UserId == userId && b.Status == Models.BookingStatus.Rejected)
            };

            return Ok(stats);
        }
    }
}