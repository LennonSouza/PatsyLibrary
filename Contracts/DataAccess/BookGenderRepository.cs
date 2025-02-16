using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess;

public class BookGenderRepository : Repository<BookGender, short>, IBookGenderRepository
{
    public BookGenderRepository(AppDbContext context) : base(context) { }

    public async Task<BookGender> GetByName(string name) => await _context.BookGenders.FirstOrDefaultAsync(x => x.Name == name);
}