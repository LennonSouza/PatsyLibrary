using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IBookRepository : IRepository<Book, int>
{
    Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);
    Task<IEnumerable<Book>> GetBooksByTitleAsync(string title);
}