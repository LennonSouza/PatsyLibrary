using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        // Configuração da chave primária
        builder.HasKey(d => d.DepartmentId);

        // Configura o identity autoIncrement (se necessário)
        builder.Property(d => d.DepartmentId)
               .ValueGeneratedOnAdd();

        // Configuração das propriedades
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.IsActive)
               .IsRequired()
               .HasDefaultValue(false);
    }
}