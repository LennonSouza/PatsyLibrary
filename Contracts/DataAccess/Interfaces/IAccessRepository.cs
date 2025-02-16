using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IAccessRepository : IRepository<Access, byte>
{
    Task<Access> GetByName(string name);
    Task<List<Permission>> GetPermissionsByAccessId(byte accessId);
    Task<bool> PermissionExists(byte accessId, short permissionId);
    Task AddPermissionToAccess(AccessPermission model);
    Task<AccessPermission> GetPermissionIdByAccessId(byte accessId, short permissionId);
    void RemovePermissionToAccess(AccessPermission model);
}