namespace PatsyLibrary.Models;

public sealed class Access
{
    public byte AccessId { get; private set; }
    public string Name { get; private set; }

    public ICollection<Permission> Permissions { get; set; }  // Coleção de Permissions

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
}