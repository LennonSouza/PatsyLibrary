namespace PatsyLibrary.Models;

public class Role
{
    public short RoleId { get; private set; }
    public string Name { get; private set; }

    public short DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    public byte AccessId { get; set; }
    public virtual Access Access { get; set; }

    public Role() { }
    public Role(string name, byte accessId, short departmentId)
    {
        SetName(name);
        SetAccess(accessId);
        SetDepartment(departmentId);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome da role não pode ser vazio ou nulo.");

        Name = name;
    }

    public void SetAccess(byte accessId)
    {
        if (accessId <= 0)
            throw new ArgumentException("O accessId do acesso não pode ser 0 ou menor.");

        AccessId = accessId;
    }

    public void SetDepartment(short departmentId)
    {
        if (departmentId <= 0)
            throw new ArgumentException("O departmentId do departamento não pode ser 0 ou menor.");

        DepartmentId = departmentId;
    }

    public void UpdateRole(string name, byte accessId, short departmentId)
    {
        SetName(name);
        SetAccess(accessId);
        SetDepartment(departmentId);
    }
}
