using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class DepartmentUserConfiguration : IEntityTypeConfiguration<DepartmentUser>
{
    public void Configure(EntityTypeBuilder<DepartmentUser> builder)
    {
        // Configuração da tabela de junção entre Department e ApplicationUser
        builder.HasKey(du => new { du.DepartmentId, du.UserId });  // Chaves compostas

        builder.HasOne(du => du.Department)
            .WithMany(d => d.DepartmentUsers)
            .HasForeignKey(du => du.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(du => du.User)
            .WithMany(u => u.DepartmentUsers)
            .HasForeignKey(du => du.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
