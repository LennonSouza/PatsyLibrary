using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;

namespace PatsyLibrary.Contracts.DataAccess;

public class UnitOfWorkRepository : IUnitOfWorkRepository
{
    private IPermissionRepository _permissionRepository;
    public AppDbContext _context;

    public UnitOfWorkRepository(AppDbContext context) => _context = context;

    public IPermissionRepository PermissionRepository
    {
        get => _permissionRepository = _permissionRepository ?? new PermissionRepository(_context);
    }

    public async Task Save() => await _context.SaveChangesAsync();
    public async Task Dispose() => await _context.DisposeAsync();
}