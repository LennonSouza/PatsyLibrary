namespace PatsyLibrary.Models;

public class DepartmentApplicationUser
{
    public short DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    public string ApplicationUserId { get; set; } // O ID do usuário no sistema de identidade
    public virtual ApplicationUser ApplicationUser { get; set; }
}