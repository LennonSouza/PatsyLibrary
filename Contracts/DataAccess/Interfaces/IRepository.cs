namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IRepository<T, TKey> where T : class
{
    Task Insert(T entity);
    Task InsertRange(IEnumerable<T> entities);
    Task Update(T entity);
    Task UpdateRange(IEnumerable<T> entities);
    Task Delete(T entity);
    Task DeleteRange(IEnumerable<T> entities);

    IQueryable<T> GetAll { get; }
    Task<T> GetById(TKey key);
}