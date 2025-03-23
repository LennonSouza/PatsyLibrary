namespace PatsyLibrary.Models;

public class User
{
    public int UserId { get; private set; }
    public string UserName { get; private set; }
    public string PassWord { get; private set; }
    public string Email { get; private set; }
    public bool IsActive { get; private set; }

    public short DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    public short RoleId { get; set; }
    public virtual Role Role { get; set; }

    public User() { }
    public User(string userName, string password, string email, short departmentId)
    {
        SetUserName(userName);
        SetPassword(password);
        SetEmail(email);
        SetDepartment(departmentId);
    }

    // Métodos para modificar o estado do objeto
    public void SetUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("O nome de usuário não pode ser vazio ou nulo.");

        UserName = userName;
    }

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("A senha não pode ser vazia ou nula.");

        PassWord = password;
    }

    public void SetEmail(string email)
    {
        if(string.IsNullOrWhiteSpace(email)) throw new ArgumentException("O email não pode ser vazio ou nulo.");

        Email = email;
    }

    public void SetDepartment(short departmentId)
    {
        if (departmentId < 1) throw new ArgumentException("Deve conter um departamento.");

        DepartmentId = departmentId;
    }

    public void SetRole(short roleId)
    {
        if (roleId < 1) throw new ArgumentException("Deve conter um cargo.");

        RoleId = roleId;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}