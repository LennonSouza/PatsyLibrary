using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Helpers;
using PatsyLibrary.Models;

namespace PatsyLibrary.Controllers;

public class DepartmentController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IUserService _userService;

    public DepartmentController(IUnitOfWorkRepository unitOfWorkRepository, IUserService userService)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Obtém todas as permissões disponíveis
        List<Department> todosDepartamentos = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();

        // Verifica se o usuário tem a permissão "Super Admin" (ID 000)
        if (userPermissions.Contains(000)) return View(todosDepartamentos);

        // Verifica se o usuário tem a permissão "Ver Permissões" (ID 100)
        if (userPermissions.Contains(200))
        {
            // Filtra apenas o departamento do usuário
            List<Department> departamentosDoUsuario = todosDepartamentos
                .Where(d => d.DepartmentId == user.DepartmentId)
                .ToList();

            return View(departamentosDoUsuario);
        }

        return PartialView("AccessDenied");
    }

    [HttpGet]
    public async Task<IActionResult> Insert()
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Obtém todas as permissões disponíveis
        List<Department> todosDepartamentos = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();

        // Verifica se o usuário tem a permissão "Criar Departamentos" (ID 201, supondo que seja essa)
        if (!userPermissions.Contains(201) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para adicionar departamentos." });

        // Retorna o formulário para adicionar um novo departamento
        return PartialView("_DepartmentForm", new Department());
    }

    [HttpPost]
    public async Task<IActionResult> Insert(string name, bool isActive)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Erro => Nome não pode ser vazio ou nulo." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Editar Departamentos" (ID 201, supondo que seja essa)
        if (!userPermissions.Contains(202) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar departamentos." });

        try
        {
            // Criar e adicionar o departamento
            Department department = new(name);

            if (isActive) department.Activate();
            else department.Deactivate();

            await _unitOfWorkRepository.DepartmentRepository.Insert(department);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = "Departamento adicionado com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = $"Erro inesperado => {e.InnerException?.Message ?? e.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(short departmentId)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (departmentId < 1) return Json(new { success = false, message = "Erro => Departamento não encontrada" });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Editar Departamentos" (ID 201, supondo que seja essa)
        if (!userPermissions.Contains(202) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar departamentos." });

        Department department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department is null) return Json(new { success = false, message = "Erro => Departamento não encontrada" });

        return PartialView("_DepartmentForm", department);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short departmentId, string name, bool isActive)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Nome não pode ser vazio." });
        if (departmentId < 1) return Json(new { success = false, message = "Erro => Departamento não encontrada" });

        string username = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(username)) return Json(new { success = false, message = "Acesso negado: usuário não autenticado." });

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(username);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        HashSet<short> userPermissions = user.Role.RolePermissions
            .Select(p => p.PermissionId)
            .ToHashSet();

        // Supondo que a permissão para editar departamentos seja 202 ou Super Admin (0)
        if (!userPermissions.Contains(202) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar departamentos." });

        // Buscar a permissão pelo ID
        Department department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department is null) return Json(new { success = false, message = "Departamento não encontrado." });

        try
        {
            // Atualizar o nome da permissão
            department.UpdateName(name);

            if (isActive) department.Activate();
            else department.Deactivate();

            // Atualizar no banco de dados
            await _unitOfWorkRepository.DepartmentRepository.Update(department);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = "Departamento atualizado com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = $"Erro inesperado => {e.InnerException?.Message ?? e.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short departmentId)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (departmentId < 1) return Json(new { success = false, message = "Erro => Departamento não encontrada" });

        string username = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(username)) return Json(new { success = false, message = "Acesso negado: usuário não autenticado." });

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(username);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        HashSet<short> userPermissions = user.Role.RolePermissions
            .Select(p => p.PermissionId)
            .ToHashSet();

        // Supondo que a permissão para editar departamentos seja 202 ou Super Admin (0)
        if (!userPermissions.Contains(203) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para excluir departamentos." });

        Department department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department is null) return Json(new { success = false, message = "Departamento não encontrado." });

        try
        {
            await _unitOfWorkRepository.DepartmentRepository.Delete(department);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = "Departamento Excluido com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = $"Erro inesperado => {e.InnerException?.Message ?? e.Message}" });
        }
    }
}