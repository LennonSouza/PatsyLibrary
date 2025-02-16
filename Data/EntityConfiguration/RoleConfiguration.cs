using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Configuração da chave primária
        builder.HasKey(r => r.RoleId);

        // Configura o identity autoIncrement (se necessário)
        builder.Property(r => r.RoleId)
               .ValueGeneratedOnAdd();  // Caso RoleId seja auto increment

        // Configuração das propriedades
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(255);  // Limite de tamanho da string, ajuste conforme necessário

        // Relacionamento com o Departamento
        builder.HasOne(r => r.Department)  // Cada role tem um departamento
            .WithMany(d => d.Roles)  // Um departamento pode ter muitos roles
            .HasForeignKey(r => r.DepartmentId)  // Chave estrangeira para o departamento
            .OnDelete(DeleteBehavior.Cascade);  // Deleção em cascata

        // Relacionamento com Access
        builder.HasOne(r => r.Access)  // Cada role tem um access
            .WithMany()  // Não especificamos uma coleção de roles em Access
            .HasForeignKey(r => r.AccessId)  // Chave estrangeira para o access
            .OnDelete(DeleteBehavior.Cascade);  // Deleção em cascata

        builder.Navigation(x => x.Department).AutoInclude();
        builder.Navigation(x => x.Access).AutoInclude();
    }
}