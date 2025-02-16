using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess;

public class AccessRepository : Repository<Access, byte>, IAccessRepository
{
    public AccessRepository(AppDbContext context) : base(context) { }

    public async Task<Access> GetByName(string name) => await _context.Accesses.FirstOrDefaultAsync(x => x.Name == name);


    #region Relacao Acesso e Permissão
    // Método para obter permissões associadas a um Access
    public async Task<List<Permission>> GetPermissionsByAccessId(byte accessId) => await _context.AccessPermissions
                       .Where(ap => ap.AccessId == accessId)
                       .Select(ap => ap.Permission)
                       .ToListAsync();

    public async Task<bool> Exist(byte accessId, short permissionId) => await _context.AccessPermissions
            .AnyAsync(ap => ap.AccessId == accessId && ap.PermissionId == permissionId);

    public async Task<AccessPermission> GetPermissionIdByAccessId(byte accessId, short permissionId) =>  await _context.AccessPermissions
            .FirstOrDefaultAsync(ap => ap.AccessId == accessId && ap.PermissionId == permissionId);

    public async Task AddPermissionToAccess(AccessPermission model)
    {
        await _context.AccessPermissions.AddAsync(new AccessPermission
        {
            AccessId = model.AccessId,
            PermissionId = model.PermissionId
        });
    }

    public void RemovePermissionToAccess(AccessPermission model) => _context.AccessPermissions.Remove(model);
    #endregion
}