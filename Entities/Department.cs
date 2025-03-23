namespace PatsyLibrary.Models;

public class Department
{
    public short DepartmentId { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    public virtual ICollection<Role> Roles { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Book> Books { get; set; }

    public Department() { }

    public Department(string name) => SetName(name);

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("O nome do departamento não pode ser vazio ou nulo.");

        Name = name;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}