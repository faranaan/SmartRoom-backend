namespace SmartRoom.API.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalRooms { get; set; }
        public int PendingBookings { get; set; }
        public int ActiveBookingsToday { get; set; }
    }

    public class StudentDashboardDto
    {
        public int MyActiveBookings { get; set; } 
        public int MyTotalBookings { get; set; } 
        public int MyRejectedBookings { get; set; }
    }
}