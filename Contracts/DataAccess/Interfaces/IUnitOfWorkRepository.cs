namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IUnitOfWorkRepository
{
    IPermissionRepository PermissionRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IRoleRepository RoleRepository { get; }
    IBookGenderRepository BookGenderRepository { get; }
    IBookPublisherRepository BookPublisherRepository { get; }
    IBookRepository BookRepository { get; }
    IUserRepository UserRepository { get; }
    IRolePermissionRepository  RolePermissionRepository { get; }

    Task Save();
}