namespace PatsyLibrary.Models;

public class Role
{
    public short RoleId { get; private set; }
    public string Name { get; private set; }

    public short DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    public byte AccessId { get; set; }
    public virtual Access Access { get; set; }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome da role não pode ser vazio ou nulo.");

        Name = name;
    }
}
