using HotDeskBooking.Models;

namespace HotDeskBooking.Interfaces
{
    public interface IDesks
    {
        Task<bool> AddDesk(Desk desk);
        Task<bool> DeleteDesk(int id);
        Task<Desk> GetDesk(int id);
        Task<List<Desk>> GetDesks();
        Task<List<Desk>> GetDesksByOffice(int officeId);
        Task<bool> ChangeAvailability(int id);
        Task<bool> UpdateDesk(Desk desk);
    }
}
