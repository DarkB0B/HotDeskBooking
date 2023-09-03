using HotDeskBooking.Models;

namespace HotDeskBooking.Interfaces
{
    public interface IReservations
    {
        Task AddReservation(Reservation reservation);
        Task<Reservation> CreateReservation(DateTime startDate, DateTime endDate, int deskId, string userName);
        Task RemoveReservation(int id);
        Task<List<Reservation>> GetReservations();
        Task<Reservation> GetReservation(int id);
        Task<List<Reservation>> GetReservationsByDesk(int deskId);
        Task<List<Reservation>> GetReservationsByEmployee(int employeeId);
        Task UpdateReservation(int id, int deskId);
    }
}
