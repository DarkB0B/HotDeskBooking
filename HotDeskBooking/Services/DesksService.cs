using HotDeskBooking.DataAccess;
using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotDeskBooking.Services
{
    public class DesksService : IDesks
    {
        private readonly DataContext _context;

        public DesksService(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddDesk(Desk desk)
        {
            Desk deskInDb = _context.Desks.FirstOrDefault(desk);
            if (deskInDb != null) 
            {
                throw new Exception("Desk already exists");
            }
            await _context.Desks.AddAsync(desk);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeAvailability(int id)
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
                return true;
            }
            desk.IsAvailable = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDesk(int id)
        {
            Desk desk = await GetDesk(id);
            if (desk == null)
            {
                throw new ArgumentException("Desk with this id does not exist");
            }
            if(desk.Reservations.Any(r => r.StartDate >= DateTime.Now)) 
            {
                throw new ArgumentException();
            }
            _context.Desks.Remove(desk);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Desk>> GetAvailableDesks()
        {
            return await _context.Desks.Where(d => d.IsAvailable == true).ToListAsync();
        }

        public async Task<List<Desk>> GetAvailableDesksByOffice(int officeId)
        {
            return await _context.Desks.Where(d => d.Office.Id == officeId).Where(d => d.IsAvailable == true).ToListAsync();
        }

        public async Task<Desk> GetDesk(int id)
        {
            return await _context.Desks.FindAsync(id);
        }

        public async Task<List<Desk>> GetDesks()
        {
            return await _context.Desks.ToListAsync();
        }

        public async Task<List<Desk>> GetDesksByOffice(int officeId)
        {
            return await _context.Desks.Where(d => d.Office.Id == officeId).ToListAsync();
        }


    }
}
