using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IBookPublisherRepository : IRepository<BookPublisher, short>
{
    Task<BookPublisher> GetByName(string name);
}