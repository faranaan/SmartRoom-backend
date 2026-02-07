namespace SmartRoom.API.Models
{
    public enum UserRole
    {
        Admin,
        Mahasiswa,
        Dosen
    }

    public class User{
        public int Id { get; set; }
        public string Username{ get; set; } = string.Empty;
        public string Password{ get; set; } = string.Empty;
        public UserRole Role{ get; set; } = UserRole.Mahasiswa;
    }
}