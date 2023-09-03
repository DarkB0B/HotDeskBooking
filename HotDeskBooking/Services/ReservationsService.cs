using HotDeskBooking.DataAccess;
using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotDeskBooking.Services
{
    public class ReservationsService : IReservations
    {
        private readonly DataContext _context;
        private readonly IDesks _desksService;
        private readonly IUsers _usersService;

        public ReservationsService(DataContext context, IDesks desksService, IUsers usersService)
        {
            _context = context;
            _desksService = desksService;
            _usersService = usersService;
        }
        public async Task<Reservation> CreateReservation(DateTime startDate, DateTime endDate, int deskId, string userName)
        {
            Desk desk = await _desksService.GetDesk(deskId);
            User user = await _usersService.GetUserByName(userName);
            Reservation reservation = new Reservation { StartDate = startDate, EndDate = endDate, Desk = desk, User = user };
            return reservation;
        }
        public async Task AddReservation(Reservation reservation)
        {
            Reservation reservationInDb = await _context.Reservations.Include(r => r.User).Include(r => r.Desk).FirstOrDefaultAsync(r => r.Id == reservation.Id);
            if (reservationInDb != null)
            {
                throw new ArgumentException();
            }
            if(reservation.StartDate == null || reservation.EndDate == null || reservation.User == null || reservation.Desk == null || reservation.StartDate.Date < DateTime.Now.Date)
            {
                throw new ArgumentException();
            }
            reservation.StartDate = reservation.StartDate.Date;
            reservation.EndDate = reservation.EndDate.Date;
            if ((reservation.EndDate - reservation.StartDate).TotalDays > 7)
            {
                throw new Exception("Reservation period cannot exceed one week.");
            }

            List<Reservation> reservations = await GetReservationsByDesk(reservation.Desk.Id);

            if (reservations.Any(r => r.StartDate <= reservation.StartDate && r.EndDate >= reservation.StartDate) || reservations.Any(r => r.StartDate <= reservation.EndDate && r.EndDate >= reservation.EndDate))
            {
                throw new Exception("Desk is already reserved in this time period");
            }

            if (reservation.Desk.IsAvailable == false)
            {
                throw new Exception("Desk is unavailable");
            }

            await _context.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<Reservation> GetReservation(int id)
        {
            Reservation reservation = await _context.Reservations.Include(r => r.User).Include(r => r.Desk).FirstOrDefaultAsync(r => r.Id == id);
            if(reservation == null)
            {
                throw new ArgumentException("Reservation with this Id does not exist");
            }
            return reservation;
        }

        public async Task<List<Reservation>> GetReservations()
        {
            return await _context.Reservations.Include(r => r.User).Include(r => r.Desk).ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByDesk(int deskId)
        {
            return await _context.Reservations.Where(r => r.Desk.Id == deskId).Include(r => r.User).Include(r => r.Desk).ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByEmployee(int employeeId)
        {
            return await _context.Reservations.Where(r => r.User.Id == employeeId).Include(r => r.User).Include(r => r.Desk).ToListAsync();
        }

        public async Task RemoveReservation(int id)
        {
            Reservation reservation = await GetReservation(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();  
        }

        public async Task UpdateReservation(int id, int deskId)
        {
            Reservation reservationInDb = await GetReservation(id);
            Desk desk = await _context.Desks.FindAsync(deskId);
            if(desk == null) 
            {
                throw new ArgumentException("Desk with this Id does not exist");
            }
            if (DateTime.Now >= reservationInDb.StartDate.AddHours(-24))
            {
                throw new Exception("Cannot update reservation less than 24 hours before the start date");
            }
            Reservation? isReserved = await _context.Reservations.Where(r => r.Desk.Id == deskId && r.Id != id && r.StartDate < reservationInDb.EndDate && r.EndDate > reservationInDb.StartDate).FirstOrDefaultAsync();

            if (isReserved != null) 
            {
                throw new Exception("Desk is already reserved during the requested time period");
            }
           
            reservationInDb.Desk = desk;
            _context.Reservations.Update(reservationInDb);
            await _context.SaveChangesAsync();
        }
    }
}
