using HotDeskBooking.Models;
using System.Runtime.InteropServices;

namespace HotDeskBooking.Interfaces
{
    public interface IDesks
    {
        Task<Desk> CreateDesk(int locationId);
        Task AddDesk(Desk desk);
        Task DeleteDesk(int id);
        Task<Desk> GetDesk(int id, bool isAdmin);
        Task<Desk> GetDesk(int id);
        Task<List<Desk>> GetAvailableDesks(DateTime? startDateTime, DateTime? endDateTime);
        Task<List<Desk>> GetAvailableDesksByLocation(int locationId, DateTime? startDateTime, DateTime? endDateTime);
        Task<List<Desk>> GetDesks(bool isAdmin);
        Task<List<Desk>> GetDesks();
        Task<List<Desk>> GetDesksByLocation(int locationId, bool isAdmin);
        Task<List<Desk>> GetDesksByLocation(int locationId);
        Task<string> ChangeAvailability(int id);
    }
}
