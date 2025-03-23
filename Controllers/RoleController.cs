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

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);

        IQueryable<User> users = _unitOfWorkRepository.UserRepository.GetAll;

        // Obtém os departamentos filtrados
        IQueryable<Role> rolesQuery = _unitOfWorkRepository.RoleRepository.GetAll;

        // Se o usuário não tiver o departamento "1" (Master), filtra apenas os vinculados
        if (user.DepartmentId != 1)
        {
            rolesQuery = rolesQuery.Where(d => d.DepartmentId == user.DepartmentId);
        }

        // Materializa a consulta para uma lista
        List<Role> filteredRoles = await rolesQuery.ToListAsync();

        return View(filteredRoles);
    }

    // Action para exibir o formulário de inserção
    public async Task<IActionResult> Insert()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        ViewBag.DepartmentList = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();

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

            User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);

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

        // Carregar a lista de departamentos
        ViewBag.DepartmentList = await _unitOfWorkRepository.DepartmentRepository.GetAll
            .Where(d => d.IsActive) // Opcional: apenas departamentos ativos
            .Select(d => new { d.DepartmentId, d.Name })
            .ToListAsync();

        // Buscar o cargo pelo ID
        Role role = await _unitOfWorkRepository.RoleRepository.GetById(id);
        if (role == null)
            return NotFound();

        return PartialView("_RoleForm", role);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short roleId, string name, short departmentId, bool isActive)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (string.IsNullOrWhiteSpace(name))
        {
            return Json(new { success = false, message = "Nome não pode ser vazio." });
        }

        // Buscar o cargo pelo ID
        var role = await _unitOfWorkRepository.RoleRepository.GetById(roleId);
        if (role == null)
        {
            return Json(new { success = false, message = "Cargo não encontrado." });
        }

        // Verificar se o departamento existe
        var department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department == null)
        {
            return Json(new { success = false, message = "Departamento não encontrado." });
        }

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

        Role role = await _unitOfWorkRepository.RoleRepository.GetById(id);
        if (role is null) return NotFound();

        await _unitOfWorkRepository.RoleRepository.Delete(role);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Cargo Excluido com sucesso!" });
    }

    [HttpGet]
    public async Task<IActionResult> ManagePermissions(short roleId)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        var role = await _unitOfWorkRepository.RoleRepository.GetById(roleId);
        if (role == null)
            return NotFound();

        var allPermissions = await _unitOfWorkRepository.PermissionRepository.GetAll.ToListAsync();

        return PartialView("_ManagePermissions", (Role: role, AllPermissions: allPermissions));
    }

    [HttpPost]
    public async Task<IActionResult> AddPermission(short roleId, short permissionId)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        var role = await _unitOfWorkRepository.RoleRepository.GetById(roleId);
        var permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);

        if (role == null || permission == null)
            return Json(new { success = false, message = "Cargo ou permissão não encontrados." });

        var rolePermission = new RolePermission { RoleId = roleId, PermissionId = permissionId };
        await _unitOfWorkRepository.RolePermissionRepository.Insert(rolePermission);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Permissão adicionada com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> RemovePermission(short roleId, short permissionId) // Ajuste para short
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        var rolePermission = await _unitOfWorkRepository.RolePermissionRepository.GetByIds(roleId, permissionId);
        if (rolePermission == null)
            return Json(new { success = false, message = "Permissão não associada ao cargo." });

        await _unitOfWorkRepository.RolePermissionRepository.Delete(rolePermission);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Permissão removida com sucesso!" });
    }
}