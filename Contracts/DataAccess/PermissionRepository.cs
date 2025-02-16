using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess;

public class PermissionRepository : Repository<Permission, short>, IPermissionRepository
{
    public PermissionRepository(AppDbContext context) : base(context) { }

    public IQueryable<Permission> AllNames => _context.Permissions;

    public async Task<Permission> GetByName(string name) => await _context.Permissions.FirstOrDefaultAsync(x => x.Name == name);

    public async Task Save() => await _context.SaveChangesAsync();
}