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
            .HasMaxLength(30);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(50);

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
        builder.HasOne(b => b.Department)  // Especifica explicitamente a propriedade de navegação
               .WithMany(d => d.Books)     // Especifica que um departamento pode ter muitos livros
               .HasForeignKey(b => b.DepartmentId)  // Define a chave estrangeira corretamente
               .OnDelete(DeleteBehavior.Restrict); // Impede a exclusão do departamento se houver livros associados


        // Relacionamento com o BookGender (N:1)
        builder.HasOne(b => b.BookGender)
               .WithMany(bg => bg.Books)  // Certifique-se de que BookGender tem uma coleção Books
               .HasForeignKey(b => b.BookGenderId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.Navigation(x => x.Department).AutoInclude();
        builder.Navigation(x => x.BookGender).AutoInclude();
    }
}