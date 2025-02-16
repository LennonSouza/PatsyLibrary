namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IUnitOfWorkRepository
{
    IPermissionRepository PermissionRepository { get; }
    IAccessRepository AccessRepository { get; }

    Task Save();
}