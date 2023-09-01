namespace HotDeskBooking.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public Desk Desk { get; set; }
        public User User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
