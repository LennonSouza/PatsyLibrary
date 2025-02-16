namespace PatsyLibrary.Models;

public sealed class Permission
{
    public Permission(string name)
    {
        SetName(name);
    }

    // Construtor padrão (sem parâmetros)
    public Permission() { }

    public short PermissionId { get; private set; }
    public string Name { get; private set; }

    // Relacionamento muitos-para-muitos
    public ICollection<Access> Accesses { get; set; }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome da permissão não pode ser vazio ou nulo.");

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