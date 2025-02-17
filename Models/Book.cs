namespace PatsyLibrary.Models;

public class Book
{
    public Book()
    {
        // Inicializando coleções, por exemplo
        // BookGenres = new List<BookGender>();
    }

    // Construtor para garantir que o livro seja criado com dados válidos.
    public Book(
        string author, 
        string tittle, 
        string publicationYear, 
        short amountPage, 
        string sinopse, 
        string isbn, 
        byte[] coverImage, 
        string language, 
        short departmentId, 
        short bookGenderId)
    {
        SetAuthor(author);
        SetTittle(tittle);
        SetPublicationYear(publicationYear);
        SetAmountPage(amountPage);
        SetSinopse(sinopse);
        SetIsbn(isbn);
        SetCoverImage(coverImage);
        SetLanguage(language);
        SetDepartmentId(departmentId);
        SetBookGenderId(bookGenderId);
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public Book(
     string author,
     string tittle,
     string sinopse,
     string isbn,
     short departmentId)
    {
        SetAuthor(author);
        SetTittle(tittle);
        SetSinopse(sinopse);
        SetIsbn(isbn);
        SetDepartmentId(departmentId);
        BookGenderId = null;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public int BookId { get; private set; }
    public string Author { get; private set; }
    public string Tittle { get; private set; }
    public string PublicationYear { get; private set; }
    public short AmountPage { get; private set; }
    public string Sinopse { get; private set; }
    public string ISBN { get; private set; }
    public byte[] CoverImage { get; private set; }
    public string Language { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    public short DepartmentId { get; set; }
    public virtual Department Department { get; set; }
    public short? BookGenderId { get; set; }
    public virtual BookGender BookGender { get; set; }

    // Métodos para atualizar as propriedades com validações
    public void SetAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("O autor não pode ser vazio.");

        Author = author;
    }

    public void SetTittle(string tittle)
    {
        if (string.IsNullOrWhiteSpace(tittle))
            throw new ArgumentException("O título do livro não pode ser vazio.");

        Tittle = tittle;
    }

    public void SetPublicationYear(string publicationYear)
    {
        if (string.IsNullOrWhiteSpace(publicationYear))
            throw new ArgumentException("O ano de publicação não pode ser vazio.");

        PublicationYear = publicationYear;
    }

    public void SetAmountPage(short amountPage)
    {
        if (amountPage <= 0)
            throw new ArgumentException("O número de páginas deve ser maior que zero.");

        AmountPage = amountPage;
    }

    public void SetSinopse(string sinopse)
    {
        if (string.IsNullOrWhiteSpace(sinopse))
            throw new ArgumentException("A sinopse não pode ser vazia.");

        Sinopse = sinopse;
    }

    public void SetIsbn(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("O ISBN não pode ser vazio.");

        ISBN = isbn;
    }

    public void SetCoverImage(byte[] coverImage)
    {
        if (coverImage is null || coverImage.Length == 0)
            throw new ArgumentException("A imagem de capa não pode ser vazia.");

        CoverImage = coverImage;
    }

    public void SetLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("O idioma não pode ser vazio.");

        Language = language;
    }

    public void SetDepartmentId(short departmentId)
    {
        if (departmentId <= 0)
            throw new ArgumentException("O ID do departamento deve ser válido.");

        DepartmentId = departmentId;
    }

    public void SetBookGenderId(short? bookGenderId)
    {
        if (bookGenderId is not null && bookGenderId <= 0)
            throw new ArgumentException("O ID do gênero do livro deve ser válido.");

        BookGenderId = bookGenderId;
    }

    // Método para atualizar a capa do livro
    public void UpdateCoverImage(byte[] newCoverImage)
    {
        SetCoverImage(newCoverImage);
        LastUpdatedAt = DateTime.UtcNow;
    }

    // Método para atualizar a sinopse
    public void UpdateSinopse(string newSinopse)
    {
        if (!string.IsNullOrWhiteSpace(newSinopse))
        {
            SetSinopse(newSinopse);
            LastUpdatedAt = DateTime.UtcNow;
        }
    }

    // Método para atualizar o título
    public void UpdateTittle(string newTittle)
    {
        SetTittle(newTittle);
        LastUpdatedAt = DateTime.UtcNow;
    }

    // Método para atualizar o autor
    public void UpdateAuthor(string newAuthor)
    {
        SetAuthor(newAuthor);
        LastUpdatedAt = DateTime.UtcNow;
    }
}