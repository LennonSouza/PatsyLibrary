using PatsyLibrary.Models;

namespace PatsyLibrary.Entities;

public class RolePermission
{
    public short RoleId { get; set; }
    public short PermissionId { get; set; }

    // Relacionamentos
    public virtual Role Role { get; set; }
    public virtual Permission Permission { get; set; }
}