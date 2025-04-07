namespace PatsyLibrary.Models;

public class BookStatus
{
    // Construtor para garantir que o status de um livro seja criado com dados válidos.
    public BookStatus(int bookId, string provenance, string shelf, string shelfLetter)
    {
        BookId = bookId;
        UpdateProvenance(provenance);
        UpdateShelf(shelf);
        UpdateShelfLetter(shelfLetter);
        Rating = 0;  // Inicializa avaliação como 0, se não definida
        IsAvailable = true;  // Inicializa como disponível por padrão
    }

    public int BookId { get; private set; }
    public bool IsRead { get; private set; }  // Foi lido
    public string Provenance { get; private set; }  // Procedência
    public float Rating { get; private set; }  // Avaliação (0-5)
    public string Shelf { get; private set; }  // Estante Guardada
    public string ShelfLetter { get; private set; }  // Letra da Estante
    public DateTime? ReadDate { get; private set; }  // Data da leitura (nullable)
    public string Comments { get; private set; }  // Comentários do livro
    public bool IsAvailable { get; private set; }  // Indica se o livro está disponível ou emprestado

    public virtual ICollection<Book> Books { get; set; }

    // Método para marcar o livro como lido e registrar a data da leitura.
    public void MarkAsRead(DateTime readDate)
    {
        IsRead = true;
        ReadDate = readDate;
    }

    // Método para alterar a avaliação do livro de forma segura.
    public void SetRating(float rating)
    {
        if (rating < 0 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "A avaliação deve estar entre 0 e 5.");

        Rating = rating;
    }

    // Método para adicionar um comentário ao livro
    public void AddComment(string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("O comentário não pode ser vazio.", nameof(comment));

        Comments = comment;
    }

    // Método para emprestar o livro, tornando-o indisponível e removendo o local na estante.
    public void LendBook()
    {
        IsAvailable = false;
        Shelf = null;  // Não está mais na estante
    }

    // Método para devolver o livro, tornando-o disponível novamente e colocando-o de volta na estante.
    public void ReturnBook(string shelf, string shelfLetter)
    {
        IsAvailable = true;
        UpdateShelf(shelf);
        UpdateShelfLetter(shelfLetter);
    }

    // Métodos de validação para garantir que a Procedência e a Estante sejam válidas.
    private void UpdateProvenance(string provenance)
    {
        if (string.IsNullOrWhiteSpace(provenance))
            throw new ArgumentException("A procedência não pode ser vazia.", nameof(provenance));

        Provenance = provenance;
    }

    private void UpdateShelf(string shelf)
    {
        if (!IsAvailable && shelf != null)
            throw new InvalidOperationException("Não é possível definir a estante quando o livro não está disponível.");

        Shelf = shelf;
    }

    private void UpdateShelfLetter(string shelfLetter)
    {
        if (!IsAvailable && shelfLetter != null)
            throw new InvalidOperationException("Não é possível definir a letra da estante quando o livro não está disponível.");

        ShelfLetter = shelfLetter;
    }
}