using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;
using PatsyLibrary.ViewModels;
using System.Threading.Tasks;

namespace PatsyLibrary.Controllers;

public class AccessController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public AccessController(IUnitOfWorkRepository unitOfWorkRepository) => _unitOfWorkRepository = unitOfWorkRepository;

    // Action para exibir a página inicial (index)
    public IActionResult Index()
    {
        var accesses = _unitOfWorkRepository.AccessRepository.GetAll.ToList();
        return View(accesses);
    }

    // Action para exibir o formulário de inserção
    public IActionResult Insert()
    {
        // Retorna a view parcial para o modal de adicionar
        return PartialView("_AccessForm", new Access());
    }

    // Action para processar o formulário de inserção
    [HttpPost]
    public async Task<IActionResult> Insert(string name)
    {
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Criar e adicionar o acesso
                Access access = new(name);
                await _unitOfWorkRepository.AccessRepository.Insert(access);
                await _unitOfWorkRepository.Save();
                return Json(new { success = true, message = "Acesso adicionada com sucesso!" });
            }
        }
       
        // Se o nome for inválido, retorna falha
        return Json(new { success = false, message = "Erro ao adicionar acesso. Tente novamente." });
    }

    [HttpGet]
    public async Task<IActionResult> Update(byte id)
    {
        Access access = await _unitOfWorkRepository.AccessRepository.GetById(id);
        if (access is null) return NotFound();
        return PartialView("_AccessForm", access);
    }

    [HttpPost]
    public async Task<IActionResult> Update(byte accessId, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) // Valida o nome
        {
            return Json(new { success = false, message = "Nome não pode ser vazio." });
        }

        // Buscar a permissão pelo ID
        var access = await _unitOfWorkRepository.AccessRepository.GetById(accessId);
        if (access == null)
        {
            return Json(new { success = false, message = "Acesso não encontrado." });
        }

        // Atualizar o nome da permissão
        access.UpdateName(name);

        // Atualizar no banco de dados
        await _unitOfWorkRepository.AccessRepository.Update(access);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Acesso atualizado com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(byte id)
    {
        Access access = await _unitOfWorkRepository.AccessRepository.GetById(id);
        if (access is null) return NotFound();

        await _unitOfWorkRepository.AccessRepository.Delete(access);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Acesso Excluido com sucesso!" });
    }


    // Action para listar o acesso e as permissões disponíveis para vincular
    public async Task<IActionResult> AddPermissions(byte accessId)
    {
        var access = await _unitOfWorkRepository.AccessRepository.GetById(accessId);
        if (access is null)
        {
            return NotFound();
        }

        var permissions = await _unitOfWorkRepository.PermissionRepository.GetAll.ToListAsync(); // Buscar todas as permissões disponíveis
        var existingPermissions = await _unitOfWorkRepository.AccessRepository.GetPermissionsByAccessId(accessId); // Permissões já associadas ao acesso

        var viewModel = new AccessPermissionsViewModel
        {
            Access = access,
            AvailablePermissions = permissions.Where(p => !existingPermissions.Contains(p)).ToList(),
            SelectedPermissions = existingPermissions
        };

        return View(viewModel);
    }

    // Action para salvar as permissões vinculadas
    [HttpPost]
    public async Task<IActionResult> AddPermissions(byte accessId, List<short> permissionIds)
    {
        var access = await _unitOfWorkRepository.AccessRepository.GetById(accessId);
        if (access == null)
        {
            return NotFound();
        }

        // Adiciona as permissões selecionadas ao acesso
        foreach (var permissionId in permissionIds)
        {
            if (!await _unitOfWorkRepository.AccessRepository.Exist(accessId, permissionId))
            {
                var model = new AccessPermission
                {
                    AccessId = accessId,
                    PermissionId = permissionId
                };

                await _unitOfWorkRepository.AccessRepository.AddPermissionToAccess(model);
                await _unitOfWorkRepository.Save();
            }
        }

        await _unitOfWorkRepository.Save(); // Salva as alterações no banco de dados

        return RedirectToAction("AddPermissions", new { accessId });
    }

    [HttpPost]
    public async Task<IActionResult> RemovePermission(byte accessId, short permissionId)
    {
        var access = await _unitOfWorkRepository.AccessRepository.GetById(accessId);
        var permission = await _unitOfWorkRepository.PermissionRepository.GetById(permissionId);

        if (access != null && permission != null)
        {
            // Remover a permissão associada ao acesso
            var accessPermission = await _unitOfWorkRepository.AccessRepository.GetPermissionIdByAccessId(accessId, permissionId);

            if (accessPermission != null)
            {
                _unitOfWorkRepository.AccessRepository.RemovePermissionToAccess(accessPermission);
                await _unitOfWorkRepository.Save();
            }
        }

        return RedirectToAction("AddPermissions", new { accessId });
    }
}