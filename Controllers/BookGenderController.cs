using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;
using PatsyLibrary.Services;

namespace PatsyLibrary.Controllers;

public class BookGenderController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public BookGenderController(IUnitOfWorkRepository unitOfWorkRepository) => _unitOfWorkRepository = unitOfWorkRepository;

    // Action para exibir a página inicial (index)
    public async Task<IActionResult> Index()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        var bookGenders = _unitOfWorkRepository.BookGenderRepository.GetAll.ToList();
        return View(bookGenders);
    }

    // Action para exibir o formulário de inserção
    public async Task<IActionResult> Insert()
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });
        // Retorna a view parcial para o modal de adicionar
        return PartialView("_BookGenderForm", new BookGender());
    }

    // Action para processar o formulário de inserção
    [HttpPost]
    public async Task<IActionResult> Insert(string name)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Criar e adicionar o acesso
                BookGender bookGender = new(name);
                await _unitOfWorkRepository.BookGenderRepository.Insert(bookGender);
                await _unitOfWorkRepository.Save();
                return Json(new { success = true, message = "Genero de Livro adicionada com sucesso!" });
            }
        }

        // Se o nome for inválido, retorna falha
        return Json(new { success = false, message = "Erro ao adicionar Genero de Livro. Tente novamente." });
    }

    [HttpGet]
    public async Task<IActionResult> Update(short id)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        BookGender bookGender = await _unitOfWorkRepository.BookGenderRepository.GetById(id);
        if (bookGender is null) return NotFound();
        return PartialView("_BookGenderForm", bookGender);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short bookGenderId, string name)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (string.IsNullOrWhiteSpace(name)) // Valida o nome
        {
            return Json(new { success = false, message = "Nome não pode ser vazio." });
        }

        // Buscar a permissão pelo ID
        var genero = await _unitOfWorkRepository.BookGenderRepository.GetById(bookGenderId);
        if (genero == null)
        {
            return Json(new { success = false, message = "Genero não encontrado." });
        }

        // Atualizar o nome da permissão
        genero.UpdateBookGender(name);

        // Atualizar no banco de dados
        await _unitOfWorkRepository.BookGenderRepository.Update(genero);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Genero atualizado com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id)
    {
        if (!await LibraryHelper.Result.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        BookGender bookGender = await _unitOfWorkRepository.BookGenderRepository.GetById(id);
        if (bookGender is null) return NotFound();

        await _unitOfWorkRepository.BookGenderRepository.Delete(bookGender);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Genero Excluido com sucesso!" });
    }
}