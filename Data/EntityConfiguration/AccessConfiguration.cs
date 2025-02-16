using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class AccessConfiguration : IEntityTypeConfiguration<Access>
{
    public void Configure(EntityTypeBuilder<Access> builder)
    {
        // Configuração da chave primária
        builder.HasKey(a => a.AccessId);

        // Configura o identity autoIncrement
        builder.Property(a => a.AccessId)
               .ValueGeneratedOnAdd();

        // Configuração das propriedades
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(255);  // Limite de tamanho da string, ajuste conforme necessário

        builder.HasMany(a => a.Permissions)
          .WithMany(p => p.Accesses)
          .UsingEntity<AccessPermission>( // Tabela de relacionamento personalizada
              j => j.HasOne(ap => ap.Permission)
                    .WithMany()  // Não é necessário especificar o outro lado
                    .HasForeignKey(ap => ap.PermissionId),
              j => j.HasOne(ap => ap.Access)
                    .WithMany()  // Não é necessário especificar o outro lado
                    .HasForeignKey(ap => ap.AccessId),
              j => j.ToTable("AccessPermissions") // Define o nome da tabela de relacionamento
          );
    }
}