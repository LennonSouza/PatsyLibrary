using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        // Configuração da chave primária
        builder.HasKey(b => b.BookId);

        // Configuração de identidade auto-incrementável
        builder.Property(b => b.BookId)
               .ValueGeneratedOnAdd();

        // Configuração das propriedades
        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.Tittle)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.PublicationYear)
            .HasMaxLength(4);  // Para o ano, max length de 4 caracteres

        builder.Property(b => b.AmountPage);

        builder.Property(b => b.Sinopse)
            .IsRequired()
            .HasMaxLength(5000);  // Limite para a sinopse (ajuste conforme necessário)

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(13);  // Limite de 13 para o ISBN

        builder.Property(b => b.Language)
            .HasMaxLength(30);  // Limite para o idioma

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.LastUpdatedAt)
            .IsRequired();

        // Relacionamento com o Department (1:N)
        builder.HasOne<Department>()
            .WithMany()  // Um Department pode ter muitos Books
            .HasForeignKey(b => b.DepartmentId)  // Chave estrangeira
            .OnDelete(DeleteBehavior.Cascade);  // Restrição no caso de deleção (ajuste conforme necessidade)

        // Relacionamento opcional com o BookGender (N:1)
        builder.HasOne<BookGender>()
            .WithMany()  // Um BookGender pode ter muitos Books
            .HasForeignKey(b => b.BookGenderId)  // Chave estrangeira
            .OnDelete(DeleteBehavior.SetNull);  // Quando o BookGender for deletado, o BookGenderId será nulo
    }
}