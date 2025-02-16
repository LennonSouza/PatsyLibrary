using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IBookGenderRepository : IRepository<BookGender, short>
{
    Task<BookGender> GetByName(string name);
}