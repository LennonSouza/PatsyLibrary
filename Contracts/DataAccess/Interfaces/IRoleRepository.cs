using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IRoleRepository : IRepository<Role, short>
{
    Task<Role> GetByName(string name);
    Task<List<Role>> GetByDepartments(short departmentId);
}