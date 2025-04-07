using PatsyLibrary.Entities;

namespace PatsyLibrary.Models;

public class Role : GenericState
{
    public short RoleId { get; private set; }
    public string Name { get; private set; }

    public short DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }

    public Role() { }
    public Role(string name, short departmentId)
    {
        UpdateName(name);
        UpdateDepartment(departmentId);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("O nome da role não pode ser vazio ou nulo.");

        Name = name;
    }

    public void UpdateDepartment(short departmentId)
    {
        if (departmentId <= 0) throw new ArgumentException("O departmentId do departamento não pode ser 0 ou menor.");

        DepartmentId = departmentId;
    }
}