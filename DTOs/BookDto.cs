using System.ComponentModel.DataAnnotations;

namespace PatsyLibrary.DTOs;

public class BookDto
{
    [Required(ErrorMessage = "O campo 'Autor' é obrigatório.")]
    [StringLength(255, ErrorMessage = "O nome do autor deve ter no máximo 255 caracteres.")]
    public string Author { get; set; }

    [Required(ErrorMessage = "O campo 'Título' é obrigatório.")]
    [StringLength(255, ErrorMessage = "O título deve ter no máximo 255 caracteres.")]
    public string Tittle { get; set; }

    [Range(1000, 2100, ErrorMessage = "O ano de publicação deve estar entre 1000 e 2100.")]
    public string PublicationYear { get;  set; }

    public short AmountPage { get;  set; }

    [StringLength(5000, ErrorMessage = "A sinopse deve ter no máximo 5000 caracteres.")]
    public string Sinopse { get; set; }

    [Required(ErrorMessage = "O campo 'ISBN' é obrigatório.")]
    [StringLength(13, ErrorMessage = "O ISBN deve ter no máximo 13 caracteres.")]
    public string Isbn { get; set; }

    public string Language { get;  set; }

    [Required(ErrorMessage = "O campo 'Departamento' é obrigatório.")]
    public short DepartmentId { get; set; }
    public short BookGenderId { get; set; }
    public short BookPublisherId { get; set; }
}
