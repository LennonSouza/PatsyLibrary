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

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);

        if(user.RoleId != 1) return PartialView("AccessDenied");

        IQueryable<User> users = _unitOfWorkRepository.UserRepository.GetAll;

        // Materializa a consulta para uma lista
        List<User> filteredUsers = await users.ToListAsync();

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Insert()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);

        // Obtém os departamentos filtrados
        IQueryable<Department> departmentsQuery = _unitOfWorkRepository.DepartmentRepository.GetAll.Where(d => d.IsActive);
        if (user.DepartmentId != 1) // Se não for Master
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
        if (user.DepartmentId != 1) // Se não for Master, filtra por departamento
        {
            rolesQuery = rolesQuery.Where(r => r.DepartmentId == user.DepartmentId);
        }

        var roles = rolesQuery
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

        var (isSuccess, message) = await _userService.RegisterUserAsync(model);

        if (isSuccess)
        {
            TempData["SuccessMessage"] = message;
            return Json(new { success = true, message = "Usuário adicionado com sucesso!" });
        }

        // Em caso de erro, recarregar os departamentos e cargos
        string usename = _userService.GetUserSession();
        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        IQueryable<Department> departmentsQuery = _unitOfWorkRepository.DepartmentRepository.GetAll.Where(d => d.IsActive);
        IQueryable<Role> rolesQuery = _unitOfWorkRepository.RoleRepository.GetAll.Where(r => r.IsActive);

        if (user.DepartmentId != 1)
        {
            departmentsQuery = departmentsQuery.Where(d => d.DepartmentId == user.DepartmentId);
            rolesQuery = rolesQuery.Where(r => r.DepartmentId == user.DepartmentId);
        }

        model.Departments = departmentsQuery
            .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name })
            .ToList();
        model.Roles = rolesQuery
            .Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.Name })
            .ToList();

        return Json(new { success = false, message = "Erro ao adicionar o usuário. Tente novamente." });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        var user = await _unitOfWorkRepository.UserRepository.GetById(id);
        if (user == null)
            return NotFound();

        string currentUsername = _userService.GetUserSession();
        User currentUser = await _unitOfWorkRepository.UserRepository.GetbyUserName(currentUsername);
        if (currentUser == null)
            return Json(new { success = false, message = "Usuário logado não encontrado." });

        // Obtém os departamentos filtrados
        IQueryable<Department> departmentsQuery = _unitOfWorkRepository.DepartmentRepository.GetAll.Where(d => d.IsActive);
        if (currentUser.DepartmentId != 1) // Se não for Master
        {
            departmentsQuery = departmentsQuery.Where(d => d.DepartmentId == currentUser.DepartmentId);
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
        if (user.DepartmentId != 1) // Se não for Master, filtra por departamento
        {
            rolesQuery = rolesQuery.Where(r => r.DepartmentId == user.DepartmentId);
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
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            DepartmentId = user.DepartmentId,
            IsActive = user.IsActive,
            Departments = departments,
            Roles = roles,
            RoleId = user.RoleId
        };

        return PartialView("_UserForm", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(RegisterUserViewModel model)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        // Ignorar validação de Password e ConfirmPassword se estiverem vazios na atualização
        if (model.UserId != 0) // Atualização
        {
            ModelState.Remove("Password"); // Remove validação de Password
            ModelState.Remove("ConfirmPassword"); // Remove validação de ConfirmPassword
        }

        if (!ModelState.IsValid)
        {
            // Repopula os departamentos em caso de erro
            string currentUsername = _userService.GetUserSession();
            User currentUser = await _unitOfWorkRepository.UserRepository.GetbyUserName(currentUsername);
            IQueryable<Department> departmentsQuery = _unitOfWorkRepository.DepartmentRepository.GetAll.Where(d => d.IsActive);
            if (currentUser != null && currentUser.DepartmentId != 1)
            {
                departmentsQuery = departmentsQuery.Where(d => d.DepartmentId == currentUser.DepartmentId);
            }

            model.Departments = departmentsQuery
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name
                })
                .ToList();

            return PartialView("_UserForm", model);
        }

        User user = await _unitOfWorkRepository.UserRepository.GetById(model.UserId);
        if (user == null)
            return Json(new { success = false, message = "Usuário não encontrado." });

        // Atualiza os dados
        user.SetUserName(model.UserName);
        user.SetEmail(model.Email);
        if (!string.IsNullOrEmpty(model.Password)) // Só atualiza a senha se fornecida
        {
            user.SetPassword(model.Password);
        }
        user.DepartmentId = model.DepartmentId;
        if (model.IsActive)
            user.Activate();
        else
            user.Deactivate();

        user.RoleId = model.RoleId;

        await _unitOfWorkRepository.UserRepository.Update(user);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Usuário atualizado com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        User user = await _unitOfWorkRepository.UserRepository.GetById(id);
        if (user is null) return NotFound();

        await _unitOfWorkRepository.UserRepository.Delete(user);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Usuario Excluido com sucesso!" });
    }
}