using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Models;
using PatsyLibrary.Services;

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
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        if (user.RoleId != 1) return PartialView("AccessDenied");

        List<Department> department = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();
        return View(department);
    }

    [HttpGet]
    public async Task<IActionResult> Insert()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        if (user.RoleId != 1) return PartialView("AccessDenied");

        return PartialView("_DepartmentForm", new Department());
    }

    [HttpPost]
    public async Task<IActionResult> Insert(string name, bool isActive)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Erro => Nome não pode ser vazio ou nulo." });

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
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (departmentId < 1) return Json(new { success = false, message = "Erro => Departamento não encontrada" });

        string usename = _userService.GetUserSession();

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        if (user.RoleId != 1) return PartialView("AccessDenied");

        Department department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department is null) return Json(new { success = false, message = "Erro => Departamento não encontrada" });

        return PartialView("_DepartmentForm", department);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short departmentId, string name, bool isActive)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Nome não pode ser vazio." });
        if (departmentId < 1) return Json(new { success = false, message = "Erro => Departamento não encontrada" });

        // Buscar a permissão pelo ID
        Department department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department is null) return Json(new { success = false, message = "Departamento não encontrado." });

        try
        {
            // Atualizar o nome da permissão
            department.SetName(name);

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
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (departmentId < 1) return Json(new { success = false, message = "Erro => Departamento não encontrada" });

        Department department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department is null) return NotFound();

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