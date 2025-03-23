using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Models;
using PatsyLibrary.Services;

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
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        string usename = _userService.GetUserSession();

        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(usename);
        if (user is null) return Json(new { success = false, message = "Erro => Usuário não encontrado." });

        List<Permission> permissoes = _unitOfWorkRepository.PermissionRepository.GetAll.ToList();

        bool hasViewPermission = user.Role.RolePermissions.Any(p => p.PermissionId == 2);

        if (user.RoleId == 1) return View(permissoes);

        HashSet<short> userPermissions = user.Role.RolePermissions.Select(p => p.PermissionId).ToHashSet();

        // Pega apenas as 5 primeiras permissões da lista
        List<Permission> primeirasCinco = permissoes.Take(5).ToList();

        // Filtra as permissões que o usuário não tem
        List<Permission> permissoesRemover = primeirasCinco
            .Where(p => !userPermissions.Contains(p.PermissionId))
            .ToList();

        // Remove as permissões filtradas da lista original
        permissoes.RemoveAll(p => permissoesRemover.Contains(p));

        if (hasViewPermission) return View(permissoes);

        return PartialView("AccessDenied");
    }

    [HttpGet]
    // Exibe a página para adicionar uma nova permissão
    public async Task<IActionResult> Insert()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });



        return PartialView("_PermissionForm", new Permission());
    }

    // Adiciona uma nova permissão
    [HttpPost]
    public async Task<IActionResult> Insert(string name)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Erro => Nome vazio ou nulo. Tente novamente." });

        try
        {
            Permission permission = new(name);

            await _unitOfWorkRepository.PermissionRepository.Insert(permission);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = "Permissão adicionada com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = $"Erro inesperado => {e.InnerException?.Message ?? e.Message}" });
        }
    }

    // Exibe a página para atualizar uma permissão existente
    [HttpGet]
    public async Task<IActionResult> Update(short permissionId)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (permissionId < 1) return Json(new { success = false, message = "Erro => Permissão não encontrada" });

        Permission permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);
        if (permission is null) return Json(new { success = false, message = "Erro => Permissão não encontrada" });

        return PartialView("_PermissionForm", permission);
    }

    // Atualiza um permissão existente
    [HttpPost]
    public async Task<IActionResult> Update(short permissionId, string name)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado. A sessão expirou." });

        if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Nome não pode ser vazio." });
        if (permissionId < 1) return Json(new { success = false, message = "Erro => Permissão não encontrada" });

        // Buscar a permissão pelo ID
        Permission permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);
        if (permission is null) return Json(new { success = false, message = "Permissão não encontrada." });

        try
        {
            // Atualizar o nome da permissão
            permission.SetName(name);

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
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

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