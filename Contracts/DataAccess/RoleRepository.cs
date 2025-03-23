using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess;

public class RoleRepository : Repository<Role, short>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }
    public async Task<Role> GetByName(string name) => 
        await _context.Roles.FirstOrDefaultAsync(x => x.Name == name);

    public async Task<List<Role>> GetByDepartments(short departmentId) => 
        await _context.Roles.Where(x => x.DepartmentId == departmentId).ToListAsync();
}