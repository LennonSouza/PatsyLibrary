using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess;

public class UserRepository : Repository<User, int>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public Task<User> GetbyEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        return _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }

    public Task<User> GetbyUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) return null;

        return _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName.ToLower());
    }
}