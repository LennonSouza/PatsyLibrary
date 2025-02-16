namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IUnitOfWorkRepository
{
    IPermissionRepository PermissionRepository { get; }
    IAccessRepository AccessRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IRoleRepository RoleRepository { get; }

    Task Save();
}