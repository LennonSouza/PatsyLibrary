using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Entities;
using PatsyLibrary.Models;
using PatsyLibrary.Services;

namespace PatsyLibrary.Controllers;

public class RoleController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IUserService _userService;

    public RoleController(IUnitOfWorkRepository unitOfWorkRepository, IUserService userService)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
        _userService = userService;
    }

    // Action para exibir a página inicial (index)
    public async Task<IActionResult> Index()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Obtém todas as permissões disponíveis
        List<Role> todosCargos = await _unitOfWorkRepository.RoleRepository.GetAll.ToListAsync();

        // Verifica se o usuário tem a permissão "Super Admin" (ID 000)
        if (userPermissions.Contains(000)) return View(todosCargos);

        // Verifica se o usuário tem a permissão "Ver Permissões" (ID 300)
        if (userPermissions.Contains(300))
        {
            // Filtra apenas o departamento do usuário
            List<Role> departamentosDoUsuario = todosCargos
                .Where(d => d.DepartmentId == user.DepartmentId)
                .ToList();

            return View(departamentosDoUsuario);
        }

        return PartialView("AccessDenied");
    }

    // Action para exibir o formulário de inserção
    public async Task<IActionResult> Insert()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 301, supondo que seja essa)
        if (!userPermissions.Contains(301) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para adicionar cargos." });

        // Se o usuário for Super Admin (ID 000), pode ver todos os departamentos
        if (!userPermissions.Contains(000))
        {
            ViewBag.DepartmentList = _unitOfWorkRepository.DepartmentRepository.GetAll.Where(d => d.DepartmentId == user.DepartmentId).ToList();
        }
        else
        {
            ViewBag.DepartmentList = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();
        }

        // Retorna a view parcial para o modal de adicionar
        return PartialView("_RoleForm", new Role());
    }

    // Action para processar o formulário de inserção
    [HttpPost]
    public async Task<IActionResult> Insert(string name, short departmentId, bool isActive)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (ModelState.IsValid)
        {
            string usename = _userService.GetUserSession();
            if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

            User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
            if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

            // Obtém as permissões do usuário a partir do seu papel
            HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

            // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 301, supondo que seja essa)
            if (!userPermissions.Contains(301) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para adicionar cargos." });

            if (!string.IsNullOrWhiteSpace(name))
            {
                // Criar e adicionar o acesso
                Role role = new(name, departmentId);
                if (isActive) role.Activate();
                else role.Deactivate();

                await _unitOfWorkRepository.RoleRepository.Insert(role);
                await _unitOfWorkRepository.Save();
                return Json(new { success = true, message = "Cargo adicionado com sucesso!" });
            }
        }

        // Se o nome for inválido, retorna falha
        return Json(new { success = false, message = "Erro ao adicionar o cargo. Tente novamente." });
    }

    [HttpGet]
    public async Task<IActionResult> Update(short id)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 302, supondo que seja essa)
        if (!userPermissions.Contains(302) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar cargos." });

        // Carregar a lista de departamentos
        ViewBag.DepartmentList = await _unitOfWorkRepository.DepartmentRepository.GetAll
            .Where(d => d.IsActive) // Opcional: apenas departamentos ativos
            .Select(d => new { d.DepartmentId, d.Name })
            .ToListAsync();

        // Buscar o cargo pelo ID
        Role role = await _unitOfWorkRepository.RoleRepository.GetById(id);
        if (role is null) return Json(new { success = false, message = "Cargo não encontrado." });

        return PartialView("_RoleForm", role);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short roleId, string name, short departmentId, bool isActive)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Nome não pode ser vazio." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 302, supondo que seja essa)
        if (!userPermissions.Contains(302) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar cargos." });

        // Buscar o cargo pelo ID
        var role = await _unitOfWorkRepository.RoleRepository.GetById(roleId);
        if (role is null) return Json(new { success = false, message = "Cargo não encontrado." });

        // Verificar se o departamento existe
        var department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department is null) return Json(new { success = false, message = "Departamento não encontrado." });

        // Atualizar os dados do cargo
        role.SetName(name);
        role.SetDepartment(departmentId);

        if (isActive) role.Activate();
        else role.Deactivate();

        // Salvar as alterações
        await _unitOfWorkRepository.RoleRepository.Update(role);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Cargo atualizado com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(byte id)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 302, supondo que seja essa)
        if (!userPermissions.Contains(303) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para excluir cargos." });

        Role role = await _unitOfWorkRepository.RoleRepository.GetById(id);
        if (role is null) return Json(new { success = false, message = "Cargo não encontrado." });

        await _unitOfWorkRepository.RoleRepository.Delete(role);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Cargo Excluido com sucesso!" });
    }

    [HttpGet]
    public async Task<IActionResult> ManagePermissions(short roleId)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 302, supondo que seja essa)
        if (!userPermissions.Contains(305) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar permissões cargos." });

        var role = await _unitOfWorkRepository.RoleRepository.GetById(roleId);
        if (role is null) return Json(new { success = false, message = "Cargo não encontrado." });

        // Filtra apenas as permissões que o usuário tem OU que estão na casa dos 500
        List<Permission> allPermissions = await _unitOfWorkRepository.PermissionRepository
            .GetAll
            .Where(p => userPermissions.Contains(p.PermissionId) || (p.PermissionId >= 500 && p.PermissionId < 600))
            .ToListAsync();

        return PartialView("_ManagePermissions", (Role: role, AllPermissions: allPermissions));
    }

    [HttpPost]
    public async Task<IActionResult> AddPermission(short roleId, short permissionId)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 302, supondo que seja essa)
        if (!userPermissions.Contains(305) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar permissões cargos." });

        Role role = await _unitOfWorkRepository.RoleRepository.GetById(roleId);
        if (role is null) return Json(new { success = false, message = "Cargo não encontrados." });

        Permission permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);
        if (permission is null) return Json(new { success = false, message = "Permissão não encontrados." });

        RolePermission rolePermission = new RolePermission { RoleId = roleId, PermissionId = permissionId };
        await _unitOfWorkRepository.RolePermissionRepository.Insert(rolePermission);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Permissão adicionada com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> RemovePermission(short roleId, short permissionId) // Ajuste para short
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 302, supondo que seja essa)
        if (!userPermissions.Contains(305) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar permissões cargos." });

        RolePermission rolePermission = await _unitOfWorkRepository.RolePermissionRepository.GetByIds(roleId, permissionId);
        if (rolePermission is null) return Json(new { success = false, message = "Permissão não associada ao cargo." });

        await _unitOfWorkRepository.RolePermissionRepository.Delete(rolePermission);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Permissão removida com sucesso!" });
    }
}