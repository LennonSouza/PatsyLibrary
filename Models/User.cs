namespace PatsyLibrary.Models;

public class User
{
    public int UserId { get; private set; }
    public string UserName { get; private set; }
    public string PassWord { get; private set; }
    public string Email { get; private set; }

    // Adicionando a propriedade de navegação para o relacionamento N:N
    public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }

    // Construtor necessário para o EF Core
    private User() { }

    // Construtor
    public User(string userName, string password, string email)
    {
        SetUserName(userName);
        SetPassword(password);
        SetEmail(email);
    }

    // Métodos para modificar o estado do objeto
    public void SetUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("O nome de usuário não pode ser vazio ou nulo.");

        UserName = userName;
    }

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("A senha não pode ser vazia ou nula.");

        // Aqui você pode adicionar um hash da senha antes de armazená-la.
        PassWord = password;
    }

    public void SetEmail(string email)
    {
        if(string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O email não pode ser vazio ou nulo.");

        Email = email;
    }
}