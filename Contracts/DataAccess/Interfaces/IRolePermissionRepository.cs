using PatsyLibrary.Entities;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IRolePermissionRepository : IRepository<RolePermission, short>
{
    public Task<RolePermission> GetByIds(short roleId, short permissionId);
}