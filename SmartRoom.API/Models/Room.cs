namespace SmartRoom.API.Models
{
    public enum RoomType
    {
        Classroom,
        Laboratory,
        MeetingRoom,
        Auditorium
    }
    public enum BuildingType
    {
        TowerA,
        TowerB,
        TowerC
    }

    public class Room
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; } = 0;
        public RoomType Type { get; set; } = RoomType.Classroom;
        public BuildingType Building { get; set; } = BuildingType.TowerA;
        public bool IsAvailable { get; set; } = true;  
    }
}