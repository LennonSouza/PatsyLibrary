using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IPermissionRepository : IRepository<Permission, short>
{
    Task<Permission> GetByName(string name);
}