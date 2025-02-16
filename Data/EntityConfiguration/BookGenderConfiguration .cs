using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class BookGenderConfiguration : IEntityTypeConfiguration<BookGender>
{
    public void Configure(EntityTypeBuilder<BookGender> builder)
    {
        // Configuração da chave primária
        builder.HasKey(bg => bg.BookGenderId);

        // Configura o identity autoIncrement (se necessário)
        builder.Property(bg => bg.BookGenderId)
               .ValueGeneratedOnAdd();

        // Configuração das propriedades
        builder.Property(bg => bg.Name)
            .IsRequired()
            .HasMaxLength(255);  // Limite de tamanho da string, ajuste conforme necessário
    }
}