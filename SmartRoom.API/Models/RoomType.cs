using System.Text.Json.Serialization;

namespace SmartRoom.API.Models
{
    public class RoomType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CampusId { get; set; }
        [JsonIgnore]
        public Campus? Campus { get; set; }
        [JsonIgnore]
        public ICollection<Room>? Rooms { get; set; }
    }
}