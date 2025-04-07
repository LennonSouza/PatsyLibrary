using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.Services.Interfaces;

public interface IRoleService
{
    Task<List<Role>> ActiveRoles();
}