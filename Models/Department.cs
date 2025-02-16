namespace PatsyLibrary.Models;

public class Department
{
    public short DepartmentId { get; private set; }
    public string Name { get; private set; }

    // Um departamento tem muitos roles
    public ICollection<Role> Roles { get; set; }

    // Relacionamento muitos-para-muitos com ApplicationUser
    public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }

    public Department() { }

    public Department(string name)
    {
        SetName(name);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do departamento não pode ser vazio ou nulo.");

        Name = name;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Nome não pode ser vazio.", nameof(newName));
        }

        Name = newName; // Atualiza o nome
    }
}
