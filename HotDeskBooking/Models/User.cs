namespace HotDeskBooking.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
