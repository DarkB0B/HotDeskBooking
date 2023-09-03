using HotDeskBooking.Models;

namespace HotDeskBooking.Interfaces
{
    public interface ILocations
    {
        Task AddLocation(Location location);
        Task DeleteLocation(int id);
        Task<Location> GetLocation(int id);
        Task<List<Location>> GetLocations();
    }
}
