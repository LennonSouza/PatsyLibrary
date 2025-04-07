using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public RoleService(IUnitOfWorkRepository unitOfWorkRepository)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
    }

    public async Task<List<Role>> ActiveRoles() => await _unitOfWorkRepository.RoleRepository.GetAll.Where(r => r.IsActive).ToListAsync();
}