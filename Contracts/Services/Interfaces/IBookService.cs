using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.Services.Interfaces;

public interface IBookService
{
    public Task<(Book Book, string Genre, string Publisher)> GoogleSearchBookISBNAsync(string isbn);
    public Task<(Book Book, string Publisher)> OpenLibrarySearchBookISBNAsync(string isbn);
}