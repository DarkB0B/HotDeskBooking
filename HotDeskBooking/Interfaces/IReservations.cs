using HotDeskBooking.Models;

namespace HotDeskBooking.Interfaces
{
    public interface IReservations
    {
        Task<bool> AddReservation(Reservation reservation);
        Task<bool> RemoveReservation(int id);
        Task<List<Reservation>> GetReservations();
        Task<Reservation> GetReservation(int id);
        Task<List<Reservation>> GetReservationsByDesk(int deskId);
        Task<List<Reservation>> GetReservationsByEmployee(int employeeId);
        Task<bool> UpdateReservation(int id, Desk desk);
    }
}
