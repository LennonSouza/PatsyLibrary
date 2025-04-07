using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Helpers;
using PatsyLibrary.Models;

namespace PatsyLibrary.Controllers;

public class PermissionController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IUserService _userService;

    public PermissionController(IUnitOfWorkRepository unitOfWorkRepository, IUserService userService)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
        _userService = userService;
    }

    [HttpGet]
    // Exibe todas as permissões
    public async Task<IActionResult> Index()
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();
        if(string.IsNullOrWhiteSpace(usename)) return PartialView("AccessDenied");

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        // Obtém as permissões do usuário a partir do seu papel
        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Obtém todas as permissões disponíveis
        List<Permission> todasPermissoes = _unitOfWorkRepository.PermissionRepository.GetAll.ToList();

        // Verifica se o usuário tem a permissão "Super Admin" (ID 000)
        if (userPermissions.Contains(000)) return View(todasPermissoes);

        // Verifica se o usuário tem a permissão "Ver Permissões" (ID 100)
        if (!userPermissions.Contains(100)) return PartialView("AccessDenied");

        // Filtra apenas as permissões que o usuário possui
        List<Permission> permissoesDoUsuario = todasPermissoes
            .Where(p => userPermissions.Contains(p.PermissionId))
            .ToList();

        return View(permissoesDoUsuario);
    }

    [HttpGet]
    public async Task<IActionResult> Insert()
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext))
            return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        // Cria uma nova permissão vazia
        var permission = new Permission();

        // Recupera todas as permissões existentes
        var existingPermissions = _unitOfWorkRepository.PermissionRepository.GetAll.ToList();

        // Define os intervalos de IDs com descrições
        var permissionRanges = new Dictionary<string, string>
        {
            { "0XX", "Super Admin" },
            { "1XX", "Permissões" },
            { "2XX", "Departamentos" },
            { "3XX", "Cargos (Roles)" },
            { "4XX", "Usuários" }
        };

        // Passa as permissões existentes e os intervalos para a view
        ViewData["ExistingPermissions"] = existingPermissions;
        ViewData["PermissionRanges"] = permissionRanges;

        return PartialView("_PermissionForm", permission);
    }

    // Adiciona uma nova permissão (sem alterações aqui)
    [HttpPost]
    public async Task<IActionResult> Insert(short permissionId, string name)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext))
            return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "Erro => Nome vazio ou nulo. Tente novamente." });

        // Verifica se o PermissionId já existe
        var existingPermission = await _unitOfWorkRepository.PermissionRepository
            .GetAll
            .FirstOrDefaultAsync(p => p.PermissionId == permissionId);
        if (existingPermission != null)
            return Json(new { success = false, message = $"Erro => O ID {permissionId} já está em uso pela permissão '{existingPermission.Name}'." });

        try
        {
            Permission permission = new(permissionId, name);
            await _unitOfWorkRepository.PermissionRepository.Insert(permission);
            await _unitOfWorkRepository.Save();
            return Json(new { success = true, message = "Permissão adicionada com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = $"Erro inesperado => {e.InnerException?.Message ?? e.Message}" });
        }
    }

    //// Adiciona uma nova permissão
    //[HttpPost]
    //public async Task<IActionResult> Insert(short permissionId, string name)
    //{
    //    if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

    //    if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Erro => Nome vazio ou nulo. Tente novamente." });

    //    try
    //    {
    //        Permission permission = new(permissionId, name);

    //        await _unitOfWorkRepository.PermissionRepository.Insert(permission);
    //        await _unitOfWorkRepository.Save();

    //        return Json(new { success = true, message = "Permissão adicionada com sucesso!" });
    //    }
    //    catch (Exception e)
    //    {
    //        return Json(new { success = false, message = $"Erro inesperado => {e.InnerException?.Message ?? e.Message}" });
    //    }
    //}

    // Exibe a página para atualizar uma permissão existente
    [HttpGet]
    public async Task<IActionResult> Update(short permissionId)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (permissionId < 1) return Json(new { success = false, message = "Erro => Permissão não encontrada" });

        Permission permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);
        if (permission is null) return Json(new { success = false, message = "Erro => Permissão não encontrada" });

        return PartialView("_PermissionForm", permission);
    }

    // Atualiza um permissão existente
    [HttpPost]
    public async Task<IActionResult> Update(short permissionId, string name)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Nome não pode ser vazio." });
        if (permissionId < 1) return Json(new { success = false, message = "Erro => Permissão não encontrada" });

        // Buscar a permissão pelo ID
        Permission permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);
        if (permission is null) return Json(new { success = false, message = "Permissão não encontrada." });

        try
        {
            // Atualizar o nome da permissão
            permission.UpdateName(name);

            // Atualizar no banco de dados
            await _unitOfWorkRepository.PermissionRepository.Update(permission);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = "Permissão atualizada com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = $"Erro inesperado => {e.InnerException?.Message ?? e.Message}" });
        }
    }

    // Exclui uma permissão
    [HttpPost]
    public async Task<IActionResult> Delete(short permissionId)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (permissionId < 1) return Json(new { success = false, message = "Erro => Permissão não encontrada" });

        Permission permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);
        if (permission is null) return Json(new { success = false, message = "Erro => Permissão não encontrada" });

        try
        {
            await _unitOfWorkRepository.PermissionRepository.Delete(permission);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = "Permissão Excluida com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = $"Erro inesperado => {e.InnerException?.Message ?? e.Message}" });
        }
    }
}