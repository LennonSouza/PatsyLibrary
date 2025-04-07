using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.Services.Interfaces;

public interface IDepartmentService
{
    Task<List<Department>> ActiveDepartments();
    Task<Department> GetByDepartmentId(short departmentId);
}