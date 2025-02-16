using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class BookStatusConfiguration : IEntityTypeConfiguration<BookStatus>
{
    public void Configure(EntityTypeBuilder<BookStatus> builder)
    {
        // Configuração da chave primária
        builder.HasKey(bs => bs.BookId);

        // Configuração da chave estrangeira (relacionamento com o Book)
        builder.HasOne<Book>()
               .WithOne()  // Relacionamento 1:1 com o Book (supondo que um Book tenha apenas um status)
               .HasForeignKey<BookStatus>(bs => bs.BookId)  // Chave estrangeira
               .OnDelete(DeleteBehavior.Cascade);  // Definindo que ao excluir o livro, o status é excluído

        // Configuração das propriedades
        builder.Property(bs => bs.Provenance)
               .HasMaxLength(255);  // Limite de tamanho da string (ajuste conforme necessário)

        builder.Property(bs => bs.Shelf)
               .HasMaxLength(50);  // Limite de tamanho da string (ajuste conforme necessário)

        builder.Property(bs => bs.ShelfLetter)
               .HasMaxLength(10);  // Limite de tamanho da string (ajuste conforme necessário)

        builder.Property(bs => bs.Rating);

        builder.Property(bs => bs.IsAvailable);

        builder.Property(bs => bs.IsRead);

        builder.Property(bs => bs.ReadDate);

        builder.Property(bs => bs.Comments)
               .HasMaxLength(500);  // Limite de tamanho para comentários (ajuste conforme necessário)
    }
}