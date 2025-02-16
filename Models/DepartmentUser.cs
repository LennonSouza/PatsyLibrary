namespace PatsyLibrary.Models;

public class DepartmentUser
{
    public short DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    public int UserId { get; set; } // O ID do usuário no sistema de identidade
    public virtual User User { get; set; }
}