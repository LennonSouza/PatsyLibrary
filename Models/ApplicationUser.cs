using Microsoft.AspNetCore.Identity;

namespace PatsyLibrary.Models;

public class ApplicationUser : IdentityUser
{
    // Adicionando a propriedade de navegação para o relacionamento N:N
    public virtual ICollection<DepartmentApplicationUser> DepartmentApplicationUsers { get; set; }
}