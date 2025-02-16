using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Controllers;

public class PermissionController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public PermissionController(IUnitOfWorkRepository unitOfWorkRepository) => _unitOfWorkRepository = unitOfWorkRepository;

    [HttpGet]
    // Exibe todas as permissões
    public async Task<IActionResult> Index() => View(await _unitOfWorkRepository.PermissionRepository.GetAll.ToListAsync());

    [HttpGet]
    // Exibe a página para adicionar uma nova permissão
    public IActionResult Insert()
    {
        // Retorna a view parcial para o modal de adicionar
        return PartialView("_PermissionForm", new Permission());
    }

    // Adiciona uma nova permissão
    [HttpPost]
    public async Task<IActionResult> Insert(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            // Criar e adicionar a permissão
            Permission permission = new(name);
            await _unitOfWorkRepository.PermissionRepository.Insert(permission);
            await _unitOfWorkRepository.Save();
            return Json(new { success = true, message = "Permissão adicionada com sucesso!" });
        }

        // Se o nome for inválido, retorna falha
        return Json(new { success = false, message = "Erro ao adicionar permissão. Tente novamente." });
    }

    [HttpGet]
    public async Task<IActionResult> Update(short id)
    {
        Permission permission = await _unitOfWorkRepository.PermissionRepository.GetById(id);
        if (permission is null) return NotFound();
        return PartialView("_PermissionForm", permission);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short permissionId, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) // Valida o nome
        {
            return Json(new { success = false, message = "Nome não pode ser vazio." });
        }

        // Buscar a permissão pelo ID
        var permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);
        if (permission == null)
        {
            return Json(new { success = false, message = "Permissão não encontrada." });
        }

        // Atualizar o nome da permissão
        permission.UpdateName(name);

        // Atualizar no banco de dados
        await _unitOfWorkRepository.PermissionRepository.Update(permission);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Permissão atualizada com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id)
    {
        Permission permission = await _unitOfWorkRepository.PermissionRepository.GetById(id);
        if (permission is null) return NotFound();

        await _unitOfWorkRepository.PermissionRepository.Delete(permission);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Permissão Excluida com sucesso!" });
    }
}