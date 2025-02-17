using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Data;

namespace PatsyLibrary.Contracts.DataAccess;

public class UnitOfWorkRepository : IUnitOfWorkRepository
{
    private IPermissionRepository _permissionRepository;
    private IAccessRepository _accessRepository;
    private IDepartmentRepository _departmentRepository;
    private IRoleRepository _roleRepostitory;
    private IBookGenderRepository _bookGenderRepository;
    private IBookPublisherRepository _bookPublisherRepository;
    private IBookRepository _bookRepository;
    public AppDbContext _context;

    public UnitOfWorkRepository(AppDbContext context) => _context = context;

    public IPermissionRepository PermissionRepository
    {
        get => _permissionRepository = _permissionRepository ?? new PermissionRepository(_context);
    }

    public IAccessRepository AccessRepository
    {
        get => _accessRepository = _accessRepository ?? new AccessRepository(_context);
    }

    public IDepartmentRepository DepartmentRepository
    {
        get => _departmentRepository = _departmentRepository ?? new DepartmentRepository(_context);
    }

    public IRoleRepository RoleRepository
    {
        get => _roleRepostitory = _roleRepostitory ?? new RoleRepository(_context);
    }

    public IBookGenderRepository BookGenderRepository
    {
        get => _bookGenderRepository = _bookGenderRepository ?? new BookGenderRepository(_context);
    }

    public IBookPublisherRepository BookPublisherRepository
    {
        get => _bookPublisherRepository = _bookPublisherRepository ?? new BookPublisherRepository(_context);
    }

    public IBookRepository BookRepository
    {
        get => _bookRepository = _bookRepository ?? new BookRepository(_context);
    }

    public async Task Save() => await _context.SaveChangesAsync();
    public async Task Dispose() => await _context.DisposeAsync();
}