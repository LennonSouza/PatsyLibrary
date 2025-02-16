using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Configuração de chave primária (isso já é feito por padrão pelo Identity, mas é uma boa prática garantir)
        builder.HasKey(u => u.UserId);

        // Configura o identity autoIncrement (se necessário)
        builder.Property(r => r.UserId)
               .ValueGeneratedOnAdd();  // Caso UserId seja auto increment

        // Definir outras propriedades específicas que você deseja configurar
        builder.Property(u => u.UserName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.PassWord)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(100);
    }
}