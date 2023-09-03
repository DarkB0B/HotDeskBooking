namespace HotDeskBooking.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Desk> Desks { get; set; } = new List<Desk>();
    }
}
