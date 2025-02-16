using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class BookPublisherConfiguration : IEntityTypeConfiguration<BookPublisher>
{
    public void Configure(EntityTypeBuilder<BookPublisher> builder)
    {
        // Configuração da chave primária
        builder.HasKey(bp => bp.BookPublisherId);

        // Configura o identity autoIncrement (se necessário)
        builder.Property(bp => bp.BookPublisherId)
               .ValueGeneratedOnAdd();

        // Configuração das propriedades
        builder.Property(bp => bp.Name)
            .IsRequired()
            .HasMaxLength(255);  // Limite de tamanho da string, ajuste conforme necessário
    }
}