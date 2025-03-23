using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IUserRepository : IRepository<User, int>
{
    Task<User> GetbyEmail(string email);
    Task<User> GetbyUserName(string userName);
}