using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        // Configuração da chave primária
        builder.HasKey(r => r.RoleId);

        // Configura o identity autoIncrement (se necessário)
        builder.Property(r => r.RoleId)
               .ValueGeneratedOnAdd();  // Caso RoleId seja auto increment

        // Configuração das propriedades
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(255);  // Limite de tamanho da string, ajuste conforme necessário

        builder.Property(r => r.IsActive)
               .IsRequired()
               .HasDefaultValue(false);

        // Relacionamento com o Departamento
        builder.HasOne(r => r.Department)  // Cada role tem um departamento
            .WithMany(d => d.Roles)  // Um departamento pode ter muitos roles
            .HasForeignKey(r => r.DepartmentId)  // Chave estrangeira para o departamento
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com RolePermission
        builder.HasMany(r => r.RolePermissions)
               .WithOne(rp => rp.Role)
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Department).AutoInclude();
        builder.Navigation(x => x.Users).AutoInclude();
        builder.Navigation(x => x.RolePermissions).AutoInclude();
    }
}