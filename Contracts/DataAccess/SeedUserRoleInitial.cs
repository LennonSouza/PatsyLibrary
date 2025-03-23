using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Entities;
using PatsyLibrary.Models;
using PatsyLibrary.ViewModels;

namespace PatsyLibrary.Contracts.DataAccess;

public class SeedUserRoleInitial : ISeedUserRoleInitial
{
    private readonly IUserService _userService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IConfiguration _configuration;

    private string _email, _username, _password;

    public SeedUserRoleInitial(IUserService userService, IUnitOfWorkRepository unitOfWorkRepository, IConfiguration configuration)
    {
        _userService = userService;
        _unitOfWorkRepository = unitOfWorkRepository;
        _configuration = configuration;

        _email = _configuration["ConfigurationMasterUser:Email"];
        _username = _configuration["ConfigurationMasterUser:Username"];
        _password = _configuration["ConfigurationMasterUser:Password"];
    }

    public async Task SeedUsers()
    {
        User existingUser = await _unitOfWorkRepository.UserRepository.GetbyEmail(_email);
        if (existingUser is null)
        {
            // Criar uma permissão
            List<Permission> permissions = new List<Permission>
            {
                new("Super Admin"),
                new("Ver Permissões"),
                new("Ver Departamentos"),
                new("Ver Cargos"),
                new("Ver Usuarios")
            };

            // Verificar duplicatas antes de inserir
            List<Permission> existingPermissions = await _unitOfWorkRepository.PermissionRepository.GetAll.ToListAsync();
            List<Permission> permissionsToAdd = permissions.Where(p => !existingPermissions.Any(ep => ep.Name == p.Name)).ToList();

            if (permissionsToAdd.Any())
            {
                await _unitOfWorkRepository.PermissionRepository.InsertRange(permissionsToAdd);
                await _unitOfWorkRepository.Save();
            }

            // Recarregar todas as permissões padrão para obter os IDs gerados
            permissions = await _unitOfWorkRepository.PermissionRepository
                .GetAll.Where(p => permissions.Select(x => x.Name).Contains(p.Name))
                .ToListAsync();

            // Criar um departamento
            Department department = new("Administração");
            department.Activate();

            await _unitOfWorkRepository.DepartmentRepository.Insert(department);
            await _unitOfWorkRepository.Save();

            // Criar um cargo
            Role role = new("SuperAdmin", department.DepartmentId);
            role.Activate();

            await _unitOfWorkRepository.RoleRepository.Insert(role);
            await _unitOfWorkRepository.Save();

            // Relacionar o Role com todas as permissões
            List<RolePermission> rolePermissions = permissions.Select(p => new RolePermission
            {
                RoleId = role.RoleId,
                PermissionId = p.PermissionId
            }).ToList();

            await _unitOfWorkRepository.RolePermissionRepository.InsertRange(rolePermissions);
            await _unitOfWorkRepository.Save();

            // Criar um usuário
            await _userService.RegisterUserAsync(new RegisterUserViewModel
            {
                Email = _email,
                UserName = _username,
                Password = _password,
                ConfirmPassword = _password,
                DepartmentId = department.DepartmentId,
                RoleId = role.RoleId,
                IsActive = true
            });
        }
    }
}