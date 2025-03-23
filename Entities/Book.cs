namespace PatsyLibrary.Models;

public class Book
{
    public Book()
    {
        // Inicializando coleções, por exemplo
        // BookGenres = new List<BookGender>();
    }

    public Book(
    Book book,
    short bookGenderId,
    short bookPublisherId)
    {
        SetAuthor(book.Author);
        SetTittle(book.Title);
        SetPublicationYear(book.PublicationYear);
        SetAmountPage(book.AmountPage);
        SetSinopse(book.Sinopse);
        SetIsbn(book.ISBN);
        //SetCoverImage(coverImage);
        SetLanguage(book.Language);
        SetBookGenderId(bookGenderId);
        SetBookPublisherId(bookPublisherId);
    }

    // Construtor para garantir que o livro seja criado com dados válidos.
    public Book(
        string author, 
        string tittle, 
        string publicationYear, 
        short amountPage, 
        string sinopse, 
        string isbn, 
        //byte[] coverImage, 
        string language, 
        short departmentId, 
        short? bookGenderId,
        short? publisherId)
    {
        SetAuthor(author);
        SetTittle(tittle);
        SetPublicationYear(publicationYear);
        SetAmountPage(amountPage);
        SetSinopse(sinopse);
        SetIsbn(isbn);
        //SetCoverImage(coverImage);
        SetLanguage(language);
        SetDepartmentId(departmentId);
        SetBookGenderId(bookGenderId);
        SetBookPublisherId(publisherId);
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public Book(
     string author,
     string tittle,
     string sinopse,
     string isbn,
     short departmentId,
     short? bookGenderId,
        short? publisherId)
    {
        SetAuthor(author);
        SetTittle(tittle);
        SetSinopse(sinopse);
        SetIsbn(isbn);
        SetDepartmentId(departmentId);
        SetBookGenderId(bookGenderId);
        SetBookPublisherId(publisherId);
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public Book(
    string author,
    string tittle,
    string sinopse,
    string isbn)
    {
        SetAuthor(author);
        SetTittle(tittle);
        SetSinopse(sinopse);
        SetIsbn(isbn);
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public Book(
        string author,
        string tittle,
        string publicationYear,
        string sinopse,
        string isbn,
        //byte[] coverImage, 
        string language)
    {
        SetAuthor(author);
        SetTittle(tittle);
        SetPublicationYear(publicationYear);
        SetSinopse(sinopse);
        SetIsbn(isbn);
        //SetCoverImage(coverImage);
        SetLanguage(language);
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public int BookId { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string PublicationYear { get; set; }
    public short AmountPage { get; set; }
    public string Sinopse { get; set; }
    public string ISBN { get; set; }
    public byte[] CoverImage { get; set; }
    public string Language { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }

    public short DepartmentId { get; set; }
    public virtual Department Department { get; set; }
    public short? BookGenderId { get; set; }
    public virtual BookGender BookGender { get; set; }
    public short? BookPublisherId { get; set; }
    public virtual BookPublisher BookPublisher { get; set; }
    public short? BookStatusId { get; set; }
    public virtual BookStatus BookStatus { get; set; }

    // Métodos para atualizar as propriedades com validações
    public void SetAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("O autor não pode ser vazio.");

        Author = author;
    }

    public void SetTittle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("O título do livro não pode ser vazio.");

        Title = title;
    }

    public void SetPublicationYear(string publicationYear)
    {
        if (string.IsNullOrWhiteSpace(publicationYear))
            throw new ArgumentException("O ano de publicação não pode ser vazio.");

        // Se for uma data completa, extrai apenas o ano
        if (publicationYear.Length >= 4)
            publicationYear = publicationYear.Substring(0, 4);

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

    public void SetBookPublisherId(short? bookPublisherId)
    {
        if (bookPublisherId is not null && bookPublisherId <= 0)
            throw new ArgumentException("O ID do gênero do livro deve ser válido.");

        BookPublisherId = bookPublisherId;
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