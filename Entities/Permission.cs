using PatsyLibrary.Entities;

namespace PatsyLibrary.Models;

public class Permission
{
    public short PermissionId { get; private set; }
    public string Name { get; private set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }

    public Permission() { }
    public Permission(string name) => SetName(name);

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("O nome da permissão não pode ser vazio ou nulo.");

        Name = name;
    }
}