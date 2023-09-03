using HotDeskBooking.DataAccess;
using HotDeskBooking.Interfaces;
using HotDeskBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotDeskBooking.Services
{
    public class UsersService : IUsers
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UsersService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> GetUserByName(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Invalid user credentials provided.");
            }
            User user = await _context.Users.Include(u => u.Role).Include(u => u.Reservations).FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null)
            {
                throw new ArgumentException("Incorrect user data.");
            }
            return user;
        }

        public async Task<string> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Invalid user credentials provided.");
            }

            User user = await GetUserByName(username);

            if (user == null || user.Role == null)
            {
                throw new ArgumentException("Incorrect user data.");
            }

            List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim(ClaimTypes.Name, user.Username)
        };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(45),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task Register(string username, string password)
        {
            
            Role role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == 1);            
            try
            {
                User userInDb = await GetUserByName(username);
            }
            catch (Exception ex)
            {
                User newUser = new User { Username = username, Password = password, Role = role };
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
            }
            throw new ArgumentException("User with this name allready exists");
        }
        public async Task UpdateUserToAdmin(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new ArgumentException("Incorrect Id");
            }
            Role role = await _context.Roles.FindAsync(2);
            user.Role = role;
            await _context.SaveChangesAsync();
        }

    }
}
