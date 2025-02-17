using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Controllers;

public class BookController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public BookController(IUnitOfWorkRepository unitOfWorkRepository) => _unitOfWorkRepository = unitOfWorkRepository;

    // Action para exibir a página inicial (index)
    public IActionResult Index()
    {
        ViewBag.DepartmentList = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();
        ViewBag.BookGenderList = _unitOfWorkRepository.BookGenderRepository.GetAll.ToList();

        var books = _unitOfWorkRepository.BookRepository.GetAll.ToList();
        return View(books);
    }

    // Action para exibir o formulário de inserção
    public IActionResult Insert()
    {
        // Retorna a view parcial para o modal de adicionar
        return PartialView("_BookForm", new Book());
    }

    // INSERIR LIVRO AUTOMATICAMENTE (RECEBE JSON)
    [HttpPost]
    public async Task<IActionResult> InsertAutomatic([FromBody] Book book)
    {
        if (book == null)
        {
            return Json(new { success = false, message = "Os dados do livro são inválidos!" });
        }

        await _unitOfWorkRepository.BookRepository.Insert(book);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Livro adicionado automaticamente com sucesso!" });
    }

    // INSERIR LIVRO MANUALMENTE (RECEBE CAMPOS INDIVIDUAIS)
    [HttpPost]
    public async Task<IActionResult> InsertManual(string author, string tittle, string sinopse, string isbn, short departmentId)
    {
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(author) && !string.IsNullOrWhiteSpace(tittle))
            {
                Book book = new(author, tittle, sinopse, isbn, departmentId);
                await _unitOfWorkRepository.BookRepository.Insert(book);
                await _unitOfWorkRepository.Save();

                return Json(new { success = true, message = "Livro adicionado manualmente com sucesso!" });
            }
        }

        return Json(new { success = false, message = "Erro ao adicionar livro manualmente. Tente novamente." });
    }
}