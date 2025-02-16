using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class DepartmentApplicationUserConfiguration : IEntityTypeConfiguration<DepartmentApplicationUser>
{
    public void Configure(EntityTypeBuilder<DepartmentApplicationUser> builder)
    {
        // Configuração da tabela de junção entre Department e ApplicationUser
        builder.HasKey(du => new { du.DepartmentId, du.ApplicationUserId });  // Chaves compostas

        builder.HasOne(du => du.Department)
            .WithMany(d => d.DepartmentApplicationUsers)
            .HasForeignKey(du => du.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(du => du.ApplicationUser)
            .WithMany(u => u.DepartmentApplicationUsers)
            .HasForeignKey(du => du.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
