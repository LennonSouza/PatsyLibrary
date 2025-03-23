using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Services;

public static class DefaultPermissions
{
    private static List<Permission> _systemPermissionsCache;
    public static readonly List<string> SystemPermissions = new()
    {
        "Super Admin",
        "Ver Permissões",
        "Ver Departamentos",
        "Ver Cargos",
        "Ver Usuarios"
    };

    public static async Task Initialize(IUnitOfWorkRepository unitOfWork)
    {
        _systemPermissionsCache = await unitOfWork.PermissionRepository
            .GetAll.Where(p => SystemPermissions.Contains(p.Name))
            .ToListAsync();
    }

    public static bool IsSystemPermission(short permissionId) => _systemPermissionsCache.Any(p => p.PermissionId == permissionId);

    public static bool IsSystemPermission(string name) => SystemPermissions.Contains(name);
}