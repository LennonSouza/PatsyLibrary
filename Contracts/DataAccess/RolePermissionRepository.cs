using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Entities;

namespace PatsyLibrary.Contracts.DataAccess;

public class RolePermissionRepository : Repository<RolePermission, short>, IRolePermissionRepository
{
    public RolePermissionRepository(AppDbContext context) : base(context) { }

    public async Task<RolePermission> GetByIds(short roleId, short permissionId) => 
        await _context.RolePermissions.FirstOrDefaultAsync(r => r.RoleId == roleId && r.PermissionId == permissionId);
}