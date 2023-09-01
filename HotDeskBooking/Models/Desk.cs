namespace HotDeskBooking.Models
{
    public class Desk
    {
        public int Id { get; set; }
        public Office Office { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public bool IsAvailable { get; set; }
    }
}
