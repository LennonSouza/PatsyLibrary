using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Controllers;

public class DepartmentController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public DepartmentController(IUnitOfWorkRepository unitOfWorkRepository) => _unitOfWorkRepository = unitOfWorkRepository;

    public IActionResult Index()
    {
        var department = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();
        return View(department);
    }

    public IActionResult Insert()
    {
        // Retorna a view parcial para o modal de adicionar
        return PartialView("_DepartmentForm", new Department());
    }

    // Action para processar o formulário de inserção
    [HttpPost]
    public async Task<IActionResult> Insert(string name)
    {
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Criar e adicionar o departamento
                Department department = new(name);
                await _unitOfWorkRepository.DepartmentRepository.Insert(department);
                await _unitOfWorkRepository.Save();
                return Json(new { success = true, message = "Departamento adicionado com sucesso!" });
            }
        }

        // Se o nome for inválido, retorna falha
        return Json(new { success = false, message = "Erro ao adicionar o departamento. Tente novamente." });
    }

    [HttpGet]
    public async Task<IActionResult> Update(short id)
    {
        Department department = await _unitOfWorkRepository.DepartmentRepository.GetById(id);
        if (department is null) return NotFound();
        return PartialView("_DepartmentForm", department);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short departmentId, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) // Valida o nome
        {
            return Json(new { success = false, message = "Nome não pode ser vazio." });
        }

        // Buscar a permissão pelo ID
        var department = await _unitOfWorkRepository.DepartmentRepository.GetById(departmentId);
        if (department == null)
        {
            return Json(new { success = false, message = "Departamento não encontrado." });
        }

        // Atualizar o nome da permissão
        department.UpdateName(name);

        // Atualizar no banco de dados
        await _unitOfWorkRepository.DepartmentRepository.Update(department);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Departamento atualizado com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id)
    {
        Department department = await _unitOfWorkRepository.DepartmentRepository.GetById(id);
        if (department is null) return NotFound();

        await _unitOfWorkRepository.DepartmentRepository.Delete(department);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Departamento Excluido com sucesso!" });
    }
}