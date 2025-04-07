using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public DepartmentService(IUnitOfWorkRepository unitOfWorkRepository)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
    }

    public async Task<List<Department>> ActiveDepartments() => await _unitOfWorkRepository.DepartmentRepository.GetAll.Where(d => d.IsActive).ToListAsync();

    public async Task<Department> GetByDepartmentId(short departmentId) => await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
}