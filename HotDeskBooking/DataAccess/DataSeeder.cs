using HotDeskBooking.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace HotDeskBooking.DataAccess
{
    public class DataSeeder
    {
        private readonly DataContext _context;

        private string[] locations = { "Krakow", "Warszawa" };
        string[] roles = { "Admin", "Employee" };

        public DataSeeder(DataContext context)
        {
            _context = context;
        }

        public bool IsDatabaseEmpty()
        {
            return !_context.Roles.Any();
        }

        public async Task Seed()
        {
            if (IsDatabaseEmpty())
            {
                await SeedRoles();
                await SeedUsers();
                await SeedLocations();
                await SeedDesks();
            }
        }

        public async Task SeedRoles()
        {
            
            foreach (string role in roles)
            {
                await _context.Roles.AddAsync(new Role { Name = role });
            }
            await _context.SaveChangesAsync();
        }
        public async Task SeedUsers()
        {
            Role admin = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roles[0]);
            Role employee = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roles[1]);

            User[] users =
            {
                new User {  Username = "admin", Password = "admin",Role = admin },
                new User {  Username = "employee", Password = "employee", Role = employee }
            };
            foreach (User user in users)
            {
                await _context.Users.AddAsync(user);
            }
            await _context.SaveChangesAsync();
        }
        public async Task SeedLocations()
        {      
            for(int i = 0; i < locations.Length; i++) 
            { 
                await _context.Locations.AddAsync(new Location { Name = locations[i] });
            }
            await _context.SaveChangesAsync();
        }
        public async Task SeedDesks()
        {
            Location location1 = await _context.Locations.FirstOrDefaultAsync(l => l.Id == 1);
            Location location2 = await _context.Locations.FirstOrDefaultAsync(l => l.Id == 2);
            Desk[] desks = 
            {
                new Desk { IsAvailable = true, Location = location1 },
                new Desk { IsAvailable = false, Location = location1},
                new Desk { IsAvailable = true, Location = location2 },
            };
            foreach (var desk in desks)
            {
                await _context.Desks.AddAsync(desk);
            }
            await _context.SaveChangesAsync();

        }
    }
}
