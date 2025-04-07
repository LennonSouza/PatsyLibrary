using PatsyLibrary.Entities;

namespace PatsyLibrary.Models;

public class Department : GenericState
{
    public short DepartmentId { get; private set; }
    public string Name { get; private set; }

    public virtual ICollection<Role> Roles { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Book> Books { get; set; }

    public Department() { }

    public Department(string name) => UpdateName(name);

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("O nome do departamento não pode ser vazio ou nulo.");

        Name = name;
    }
}