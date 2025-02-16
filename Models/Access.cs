namespace PatsyLibrary.Models;

public sealed class Access
{
    public byte AccessId { get; private set; }
    public string Name { get; private set; }

    public ICollection<Permission> Permissions { get; set; }  // Coleção de Permissions

    public Access() { }

    // Construtor que aceita o nome e a lista de permissões
    public Access(string name)
    {
        SetName(name);
    }

    // Método para definir o nome
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do acesso não pode ser vazio ou nulo.");

        Name = name;
    }

    // Método público para alterar o nome
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Nome não pode ser vazio.", nameof(newName));
        }

        Name = newName; // Atualiza o nome
    }
}