using System.Text.Json.Serialization;

namespace SmartRoom.API.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; } = 0;
        public int RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }
        public int BuildingId { get; set; }
        public Building? Building { get; set; }
        public bool IsAvailable { get; set; } = true;  
        public int CampusId { get; set; } 
        [JsonIgnore]
        public Campus? Campus { get; set; }
        [JsonIgnore]
        public ICollection<Booking>? Bookings { get; set; }
    }
}