using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        // Configuração da chave primária
        builder.HasKey(d => d.DepartmentId);

        // Configura o identity autoIncrement (se necessário)
        builder.Property(d => d.DepartmentId)
               .ValueGeneratedOnAdd();

        // Configuração das propriedades
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(255);

        // Relacionamento um para muitos com Role
        builder.HasMany(d => d.Roles)  // Um departamento tem muitos roles
            .WithOne(r => r.Department)  // Cada role tem um único departamento
            .HasForeignKey(r => r.DepartmentId)  // Definindo a chave estrangeira
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.DepartmentUsers)
            .WithOne(du => du.Department)
            .HasForeignKey(du => du.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}