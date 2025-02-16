using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Configuração de chave primária (isso já é feito por padrão pelo Identity, mas é uma boa prática garantir)
        builder.HasKey(u => u.Id);

        // Ignorar propriedades herdadas de IdentityUser que não queremos mapear para o banco
        builder.Ignore(u => u.LockoutEnd);  // Ignorar o LockoutEnd
        builder.Ignore(u => u.LockoutEnabled);  // Ignorar o LockoutEnabled
        builder.Ignore(u => u.AccessFailedCount);  // Ignorar o AccessFailedCount
        builder.Ignore(u => u.TwoFactorEnabled);  // Ignorar o TwoFactorEnabled
        builder.Ignore(u => u.PhoneNumber);  // Ignorar o PhoneNumber
        builder.Ignore(u => u.PhoneNumberConfirmed);  // Ignorar o PhoneNumberConfirmed
        builder.Ignore(u => u.ConcurrencyStamp);  // Ignorar o ConcurrencyStamp
        builder.Ignore(u => u.SecurityStamp);  // Ignorar o SecurityStamp
        builder.Ignore(u => u.NormalizedEmail);  // Ignorar o NormalizedEmail
        builder.Ignore(u => u.EmailConfirmed);  // Ignorar o EmailConfirmed
        builder.Ignore(u => u.NormalizedUserName);  // Ignorar o NormalizedUserName

        // Definir outras propriedades específicas que você deseja configurar
        builder.Property(u => u.UserName)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(256);
    }
}