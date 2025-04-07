using PatsyLibrary.Entities;

namespace PatsyLibrary.Models;

public class Permission
{
    public short PermissionId { get; private set; }
    public string Name { get; private set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }

    public Permission() { }

    public Permission(string name)
    {
        UpdateName(name);
    }

    public Permission(short permissionId, string name)
    {
        UpdateName(name);
        UpdatePermissionId(permissionId);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("O nome da permissão não pode ser vazio ou nulo.");

        Name = name;
    }

    public void UpdatePermissionId(short permissionId)
    {
        if (permissionId < 0) throw new ArgumentException("O Id da permissão não pode ser menor que 0.");

        PermissionId = permissionId;
    }
}