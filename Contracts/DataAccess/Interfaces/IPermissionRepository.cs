using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IPermissionRepository : IRepository<Permission, short>
{
    IQueryable<Permission> AllNames { get; }

    Task<Permission> GetByName(string name);

    Task Save();
}