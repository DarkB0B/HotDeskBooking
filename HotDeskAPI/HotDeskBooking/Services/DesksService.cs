using HotDeskBooking.DataAccess;
using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotDeskBooking.Services
{
    public class DesksService : IDesks
    {
        private readonly DataContext _context;
        private readonly ILocations _locationsService;
        public DesksService(DataContext context, ILocations locationsService)
        {
            _context = context;
            _locationsService = locationsService;
        }

        public async Task AddDesk(Desk desk)
        {
            Desk deskInDb = _context.Desks.FirstOrDefault(d => d.Id == desk.Id);
            if (deskInDb != null) 
            {
                throw new Exception("Desk already exists");
            }
            await _context.Desks.AddAsync(desk);
            await _context.SaveChangesAsync();

        }

        public async Task<string> ChangeAvailability(int id)
        {
            Desk desk = await GetDesk(id);
            if (desk == null)
            {
                throw new ArgumentException("Desk with this id does not exist");
            }
            if (desk.IsAvailable ==  false)
            {
                desk.IsAvailable = true;
                await _context.SaveChangesAsync();
                return "Desk is now available";
            }
            desk.IsAvailable = false;
            await _context.SaveChangesAsync();
            return "Desk is now unavailable";
        }

        public async Task DeleteDesk(int id)
        {
            Desk desk = await GetDesk(id);
            if (desk == null)
            {
                throw new ArgumentException("Desk with this id does not exist");
            }
            if(desk.Reservations.Any(r => r.StartDate >= DateTime.Now.Date)) 
            {
                throw new ArgumentException("Cannot remove desk if there are reservations of this desk");
            }
            _context.Desks.Remove(desk);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Desk>> GetAvailableDesks(DateTime? startDateTime, DateTime? endDateTime)
        {
            if(startDateTime != null || endDateTime != null)
            {
                DateTime startDate = startDateTime.Value.Date;
                DateTime endDate = endDateTime.Value.Date;
                List<int> reservedDeskIds = await _context.Reservations.Where(r => r.StartDate.Date < endDate.Date && r.EndDate > startDate).Select(r => r.Desk.Id).ToListAsync();
                List<Desk> desks = await _context.Desks.Where(d => d.IsAvailable == true && !reservedDeskIds.Contains(d.Id)).ToListAsync();
                return desks;
            }
            return await _context.Desks.Where(d => d.IsAvailable == true).ToListAsync();
        }

        public async Task<List<Desk>> GetAvailableDesksByLocation(int locationId, DateTime? startDateTime, DateTime? endDateTime)
        {
            if (startDateTime != null || endDateTime != null)
            {
                DateTime startDate = startDateTime.Value.Date;
                DateTime endDate = endDateTime.Value.Date;
                List<int> reservedDeskIds = await _context.Reservations.Where(r => r.StartDate < endDate && r.EndDate > startDate).Select(r => r.Desk.Id).ToListAsync();
                List<Desk> desks = await _context.Desks.Where(d => d.Location.Id == locationId && d.IsAvailable == true && !reservedDeskIds.Contains(d.Id)).Include(d => d.Location).ToListAsync();
                return desks;
            }
                return await _context.Desks.Where(d => d.Location.Id == locationId).Where(d => d.IsAvailable == true).ToListAsync();
        }

        public async Task<Desk> GetDesk(int id)
        {
            Desk desk = await _context.Desks.Include(d => d.Reservations).Include(d => d.Location).FirstOrDefaultAsync(d => d.Id == id);
            if(desk == null)
            {
                throw new ArgumentException("Desk with this id does not exist");
            }
            return desk;
        }

        public async Task<Desk> GetDesk(int id, bool isAdmin)
        {
            if(isAdmin == false)
            { 
                return await GetDesk(id);
            }
            Desk desk = await _context.Desks.Include(d => d.Location).Include(d => d.Reservations).ThenInclude(r => r.User).FirstOrDefaultAsync(d => d.Id == id);
            if (desk == null)
            {
                throw new ArgumentException("Desk with this id does not exist");
            }
            return desk;
        }

        public async Task<List<Desk>> GetDesks()
        {
            return await _context.Desks.Include(d => d.Location).Include(d => d.Reservations).ToListAsync();
        }

        public async Task<List<Desk>> GetDesks(bool isAdmin)
        {
            if(isAdmin == false || isAdmin == null)
            {
                return await GetDesks();
            }
            return await _context.Desks.Include(d => d.Location).Include(d => d.Reservations).ThenInclude(r => r.User).ToListAsync();
        }

        public async Task<List<Desk>> GetDesksByLocation(int locationId)
        {
            return await _context.Desks.Where(d => d.Location.Id == locationId).Include(d => d.Location).Include(d => d.Reservations).ToListAsync();
        }

        public async Task<List<Desk>> GetDesksByLocation(int locationId, bool isAdmin)
        {
            if(isAdmin == false || isAdmin == null)
            {
                return await GetDesksByLocation(locationId);
            }
            return await _context.Desks.Where(d => d.Location.Id == locationId).Include(d => d.Location).Include(d => d.Reservations).ThenInclude(r => r.User).ToListAsync();
        }
        public async Task<Desk> CreateDesk(int locationId)
        {
            Location location = await _locationsService.GetLocation(locationId);
            Desk desk = new Desk { IsAvailable = true, Location = location };
            await _context.Desks.AddAsync(desk);
            return desk;
        }
    }
}
