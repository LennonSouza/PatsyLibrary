using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Models;
using PatsyLibrary.Services;
using PatsyLibrary.ViewModels;

namespace PatsyLibrary.Controllers;

public class UserController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IUserService _userService;

    public UserController(IUnitOfWorkRepository unitOfWorkRepository, IUserService userService)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return RedirectToAction("Login", "Account");

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Obtém todas as permissões disponíveis
        List<User> todosUsuarios = _unitOfWorkRepository.UserRepository.GetAll.ToList();

        // Verifica se o usuário tem a permissão "Super Admin" (ID 000)
        if (userPermissions.Contains(000)) return View(todosUsuarios);

        // Somente exibir para os cargos de Supervisão
        if (userPermissions.Contains(400))
        {
            // Filtra apenas o departamento do usuário
            List<User> usuariosDoMesmoDepartamento = todosUsuarios
                .Where(d => d.DepartmentId == user.DepartmentId)
                .ToList();

            return View(usuariosDoMesmoDepartamento);
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Insert()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return Json(new { success = false, message = "Você não tem permissão para adicionar usuarios." });

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Obtém todas as permissões disponíveis
        List<User> todosUsuarios = _unitOfWorkRepository.UserRepository.GetAll.ToList();

        // Verifica se o usuário tem a permissão "Criar Usuarios" (ID 401, supondo que seja essa)
        if (!userPermissions.Contains(401) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para adicionar usuarios." });

        // Obtém os departamentos filtrados
        IQueryable<Department> departmentsQuery = _unitOfWorkRepository.DepartmentRepository.GetAll.Where(d => d.IsActive);

        // Se o usuário for Super Admin (ID 000), pode ver todos os departamentos
        if (!userPermissions.Contains(000))
        {
            departmentsQuery = departmentsQuery.Where(d => d.DepartmentId == user.DepartmentId);
        }

        List<SelectListItem> departments = departmentsQuery
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.Name
            })
            .ToList();

        // Obtém os cargos filtrados
        IQueryable<Role> rolesQuery = _unitOfWorkRepository.RoleRepository.GetAll.Where(r => r.IsActive);

        if (!userPermissions.Contains(000))
        {
            rolesQuery = rolesQuery.Where(r => r.DepartmentId == user.DepartmentId);
        }

        // Exclui o cargo do usuário logado
        rolesQuery = rolesQuery.Where(r => r.RoleId != user.RoleId);

        List<SelectListItem> roles = rolesQuery
            .Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.Name
            })
            .ToList();

        RegisterUserViewModel viewModel = new RegisterUserViewModel
        {
            UserId = 0,
            Departments = departments,
            Roles = roles,
            IsActive = true
        };

        return PartialView("_UserForm", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Insert(RegisterUserViewModel model)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return Json(new { success = false, message = "Você não tem permissão para adicionar usuarios." });

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Criar Usuarios" (ID 401, supondo que seja essa)
        if (!userPermissions.Contains(401) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para adicionar usuarios." });

        var (isSuccess, message) = await _userService.RegisterUserAsync(model);

        if (isSuccess) return Json(new { success = true, message = "Usuário adicionado com sucesso!" });

        return Json(new { success = false, message = "Erro ao adicionar o usuário. Tente novamente." });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return Json(new { success = false, message = "Você não tem permissão para adicionar usuarios." });

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Editar Usuarios" (ID 401, supondo que seja essa)
        if (!userPermissions.Contains(402) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar usuarios." });

        var userUpdate = await _unitOfWorkRepository.UserRepository.GetById(id);
        if (userUpdate is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém os departamentos filtrados
        IQueryable<Department> departmentsQuery = _unitOfWorkRepository.DepartmentRepository.GetAll.Where(d => d.IsActive);
        if (!userPermissions.Contains(000)) // Se não for Master
        {
            departmentsQuery = departmentsQuery.Where(d => d.DepartmentId == user.DepartmentId);
        }

        var departments = departmentsQuery
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.Name
            })
            .ToList();

        // Obtém os cargos filtrados
        IQueryable<Role> rolesQuery = _unitOfWorkRepository.RoleRepository.GetAll.Where(r => r.IsActive);
        if (!userPermissions.Contains(000))
        {
            rolesQuery = rolesQuery.Where(r => r.DepartmentId == userUpdate.DepartmentId);
        }

        var roles = rolesQuery
            .Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.Name
            })
            .ToList();

        // Mapeia o User para o RegisterUserViewModel
        var viewModel = new RegisterUserViewModel
        {
            UserId = userUpdate.UserId,
            UserName = userUpdate.UserName,
            Email = userUpdate.Email,
            DepartmentId = userUpdate.DepartmentId,
            IsActive = userUpdate.IsActive,
            Departments = departments,
            Roles = roles,
            RoleId = userUpdate.RoleId
        };

        return PartialView("_UserForm", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(RegisterUserViewModel model)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return Json(new { success = false, message = "Você não tem permissão para adicionar usuarios." });

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Ignorar validação de Password e ConfirmPassword se estiverem vazios na atualização
        if (model.UserId != 0) // Atualização
        {
            ModelState.Remove("Password"); // Remove validação de Password
            ModelState.Remove("ConfirmPassword"); // Remove validação de ConfirmPassword
        }

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Editar Usuarios" (ID 401, supondo que seja essa)
        if (!userPermissions.Contains(402) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para editar usuarios." });

        User userExist = await _unitOfWorkRepository.UserRepository.GetAll.FirstOrDefaultAsync(u => u.UserName == model.UserName);
        if(userExist is not null)
        {
            if(userExist.UserName != model.UserName) return Json(new { success = false, message = "Usuário não pode conter esse 'UserName'." });
        }

        User userUpdate = await _unitOfWorkRepository.UserRepository.GetById(model.UserId);
        if (userUpdate is null) return Json(new { success = false, message = "Usuário não encontrado." });

        // Atualiza os dados
        userUpdate.SetUserName(model.UserName);
        userUpdate.SetEmail(model.Email);
        if (!string.IsNullOrEmpty(model.Password)) // Só atualiza a senha se fornecida
        {
            userUpdate.SetPassword(model.Password);
        }

        userUpdate.DepartmentId = model.DepartmentId;

        if (model.IsActive)
            userUpdate.Activate();
        else
            userUpdate.Deactivate();

        userUpdate.RoleId = model.RoleId;

        await _unitOfWorkRepository.UserRepository.Update(userUpdate);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Usuário atualizado com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        string usename = _userService.GetUserSession();
        if (string.IsNullOrWhiteSpace(usename)) return Json(new { success = false, message = "Você não tem permissão para adicionar usuarios." });

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Verifica se o usuário tem a permissão "Excluir Usuarios" (ID 401, supondo que seja essa)
        if (!userPermissions.Contains(403) && !userPermissions.Contains(000)) return Json(new { success = false, message = "Você não tem permissão para excluir usuarios." });

        User userUpdate = await _unitOfWorkRepository.UserRepository.GetById(id);
        if (userUpdate is null) return Json(new { success = false, message = "Usuário não encontrado." });

        await _unitOfWorkRepository.UserRepository.Delete(userUpdate);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Usuario Excluido com sucesso!" });
    }
}