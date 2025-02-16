namespace PatsyLibrary.Models;

public class Department
{
    public short DepartmentId { get; private set; }
    public string Name { get; private set; }

    // Um departamento tem muitos roles
    public ICollection<Role> Roles { get; set; }

    // Relacionamento muitos-para-muitos com ApplicationUser
    public virtual ICollection<DepartmentApplicationUser> DepartmentApplicationUsers { get; set; }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do departamento não pode ser vazio ou nulo.");

        Name = name;
    }
}
