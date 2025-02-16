using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IDepartmentRepository : IRepository<Department, short>
{
    Task<Department> GetByName(string name);
}