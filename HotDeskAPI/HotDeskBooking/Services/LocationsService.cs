using HotDeskBooking.DataAccess;
using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotDeskBooking.Services
{
    public class LocationsService : ILocations
    {
        private readonly DataContext _context;

        public LocationsService(DataContext context)
        {
            _context = context;
        }

        public async Task AddLocation(Location location)
        {
            if(string.IsNullOrEmpty(location.Name))
            {
                throw new ArgumentException("Location name cannot be empty");
            }
            Location locationInDb =  _context.Locations.FirstOrDefault(location);
            if (locationInDb != null)
            {
                throw new Exception("Location with this name allready exists");               
            }
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLocation(int id)
        {
            Location location = await GetLocation(id);
            if (location == null)
            {
                throw new ArgumentException("Invalid Location ID");
            }
            if(location.Desks.Count != 0) 
            {
                throw new ArgumentException("Cannot remove if desk exists in location");
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }

        public async Task<Location> GetLocation(int id)
        {
            Location location = await _context.Locations.Include(l => l.Desks).FirstOrDefaultAsync(l => l.Id == id);
            if(location == null)
            {
                throw new ArgumentException("Invalid Location ID");
            }
            return location;
        }

        public async Task<List<Location>> GetLocations()
        {
            return await _context.Locations.Include(l => l.Desks).ToListAsync();
        }
    }
}
