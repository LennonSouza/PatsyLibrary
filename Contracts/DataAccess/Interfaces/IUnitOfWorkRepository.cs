namespace PatsyLibrary.Contracts.DataAccess.Interfaces;

public interface IUnitOfWorkRepository
{
    IPermissionRepository PermissionRepository { get; }


    Task Save();
}