namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IUnitOfWorkRepository
{
    IPermissionRepository PermissionRepository { get; }
    IAccessRepository AccessRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IRoleRepository RoleRepository { get; }
    IBookGenderRepository BookGenderRepository { get; }
    IBookPublisherRepository BookPublisherRepository { get; }
    IBookRepository BookRepository { get; }

    Task Save();
}