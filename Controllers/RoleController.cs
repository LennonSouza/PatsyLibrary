using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Controllers;

public class RoleController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public RoleController(IUnitOfWorkRepository unitOfWorkRepository) => _unitOfWorkRepository = unitOfWorkRepository;

    // Action para exibir a página inicial (index)
    public IActionResult Index()
    {
        var roles = _unitOfWorkRepository.RoleRepository.GetAll.ToList();
        return View(roles);
    }

    // Action para exibir o formulário de inserção
    public IActionResult Insert()
    {
        ViewBag.AccessList = _unitOfWorkRepository.AccessRepository.GetAll.ToList();
        ViewBag.DepartmentList = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();

        // Retorna a view parcial para o modal de adicionar
        return PartialView("_RoleForm", new Role());
    }

    // Action para processar o formulário de inserção
    [HttpPost]
    public async Task<IActionResult> Insert(string name, byte accessId, short departmentId)
    {
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Criar e adicionar o acesso
                Role role = new(name, accessId, departmentId);
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
        ViewBag.AccessList = _unitOfWorkRepository.AccessRepository.GetAll.ToList();
        ViewBag.DepartmentList = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();

        Role role = await _unitOfWorkRepository.RoleRepository.GetById(id);
        if (role is null) return NotFound();
        return PartialView("_RoleForm", role);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short roleId, string name, byte accessId, short departmentId)
    {
        if (string.IsNullOrWhiteSpace(name)) // Valida o nome
        {
            return Json(new { success = false, message = "Nome não pode ser vazio." });
        }

        // Buscar a permissão pelo ID
        var role = await _unitOfWorkRepository.RoleRepository.GetById(roleId);
        if (role == null)
        {
            return Json(new { success = false, message = "Cargo não encontrado." });
        }

        var access = await _unitOfWorkRepository.AccessRepository.GetById(accessId);
        if (role == null)
        {
            return Json(new { success = false, message = "Acesso não encontrado." });
        }

        var department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (role == null)
        {
            return Json(new { success = false, message = "Departamento não encontrado." });
        }

        // Atualizar o nome da permissão
        role.UpdateRole(name, accessId, departmentId);

        // Atualizar no banco de dados
        await _unitOfWorkRepository.RoleRepository.Update(role);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Cargo atualizado com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(byte id)
    {
        Role role = await _unitOfWorkRepository.RoleRepository.GetById(id);
        if (role is null) return NotFound();

        await _unitOfWorkRepository.RoleRepository.Delete(role);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Cargo Excluido com sucesso!" });
    }
}