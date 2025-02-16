using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess;

public class BookPublisherRepository : Repository<BookPublisher, short>, IBookPublisherRepository
{
    public BookPublisherRepository(AppDbContext context) : base(context) { }

    public async Task<BookPublisher> GetByName(string name) => await _context.BookPublishers.FirstOrDefaultAsync(x => x.Name == name);
}