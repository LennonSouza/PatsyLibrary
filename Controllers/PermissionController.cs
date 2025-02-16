using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Controllers;

public class PermissionController : Controller
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionController(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }


    // Exibe todas as permissões
    public async Task<IActionResult> Index()
    {
        var permissions = await _permissionRepository.AllNames.ToListAsync();
        return View(permissions);
    }

    // Exibe a página para adicionar uma nova permissão
    public IActionResult Add()
    {
        return View();
    }

    // Adiciona uma nova permissão
    [HttpPost]
    public async Task<IActionResult> Add(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            // Criar e adicionar a permissão
            var permission = new Permission(name);
            await _permissionRepository.Insert(permission);
            await _permissionRepository.Save();
            return RedirectToAction(nameof(Index)); // Redireciona para a página de permissões
        }

        return View(); // Retorna a visão caso o nome esteja vazio
    }
}