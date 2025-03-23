using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess;

public class BookRepository : Repository<Book, int>, IBookRepository
{
    public BookRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author) => 
        await _context.Books.Where(x => x.Author == author).ToListAsync();

    public async Task<IEnumerable<Book>> GetBooksByTitleAsync(string title) =>
        await _context.Books.Where(x => x.Title == title).ToListAsync();


    public async Task<Book> GetBookByISBNAsync(string isbn, short departamentId) =>
        await _context.Books.FirstOrDefaultAsync(x => x.ISBN == isbn && x.DepartmentId == departamentId);
}