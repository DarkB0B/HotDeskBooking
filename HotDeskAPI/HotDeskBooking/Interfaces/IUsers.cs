﻿using HotDeskBooking.Models;

namespace HotDeskBooking.Interfaces
{
    public interface IUsers
    {
        Task Register (string username, string password);
        Task<string> Login (string username, string password);
        Task<User> GetUserByName(string username);
        Task UpdateUserToAdmin (int id);
    }
}
