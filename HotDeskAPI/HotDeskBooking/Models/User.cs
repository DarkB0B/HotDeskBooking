using System.Text.Json.Serialization;

namespace HotDeskBooking.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public Role Role { get; set; }
        [JsonIgnore]
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
