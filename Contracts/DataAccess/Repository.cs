using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;
using System.Security.Cryptography;

namespace PatsyLibrary.Contracts.DataAccess;

public class Repository<T, TKey> : IRepository<T, TKey> where T : class
{
    protected AppDbContext _context;

    public Repository(AppDbContext context) => _context = context;

    public async Task Insert(T entity) => await _context.Set<T>().AddAsync(entity);

    public async Task InsertRange(IEnumerable<T> entities) => await _context.Set<T>().AddRangeAsync(entities);

    public async Task Update(T entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _context.Set<T>().Attach(entity);
            entry.State = EntityState.Modified;
        }

        await Task.Yield();
    }

    public async Task UpdateRange(IEnumerable<T> entities)
    {
        var updadteTasks = entities.Select(entity => Update(entity));
        await Task.WhenAll(updadteTasks);
    }

    public async Task Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
        await Task.CompletedTask;
    }

    public IQueryable<T> GetAll => _context.Set<T>();

    public async Task<T> GetById(TKey key)
    {
        var keyName = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties[0].Name;

        if (string.IsNullOrWhiteSpace(keyName)) return null;

        return await _context.Set<T>().FirstOrDefaultAsync(e => EF.Property<TKey>(e, keyName).Equals(key));
    }
}