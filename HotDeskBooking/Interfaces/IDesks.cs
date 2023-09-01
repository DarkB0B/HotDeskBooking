using HotDeskBooking.Models;
using System.Runtime.InteropServices;

namespace HotDeskBooking.Interfaces
{
    public interface IDesks
    {
        Task<bool> AddDesk(Desk desk);
        Task<bool> DeleteDesk(int id);
        Task<Desk> GetDesk(int id, bool isAdmin);
        Task<Desk> GetDesk(int id);
        Task<List<Desk>> GetAvailableDesks(DateTime? startDateTime, DateTime? endDateTime);
        Task<List<Desk>> GetAvailableDesksByOffice(int officeId, DateTime? startDateTime, DateTime? endDateTime);
        Task<List<Desk>> GetDesks(bool isAdmin);
        Task<List<Desk>> GetDesks();
        Task<List<Desk>> GetDesksByOffice(int officeId, bool isAdmin);
        Task<List<Desk>> GetDesksByOffice(int officeId);
        Task<bool> ChangeAvailability(int id);
    }
}
