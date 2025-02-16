using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess;

public class PermissionRepository : Repository<Permission, short>, IPermissionRepository
{
    public PermissionRepository(AppDbContext context) : base(context) { }

    public async Task<Permission> GetByName(string name) => await _context.Permissions.FirstOrDefaultAsync(x => x.Name == name);
}