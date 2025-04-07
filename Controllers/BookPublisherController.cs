using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Helpers;
using PatsyLibrary.Models;

namespace PatsyLibrary.Controllers;

public class BookPublisherController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public BookPublisherController(IUnitOfWorkRepository unitOfWorkRepository) => _unitOfWorkRepository = unitOfWorkRepository;

    // Action para exibir a página inicial (index)
    public async Task<IActionResult> Index()
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        var bookPublishers = _unitOfWorkRepository.BookPublisherRepository.GetAll.ToList();
        return View(bookPublishers);
    }

    // Action para exibir o formulário de inserção
    public async Task<IActionResult> Insert()
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });
        // Retorna a view parcial para o modal de adicionar
        return PartialView("_BookPublisherForm", new BookPublisher());
    }

    // Action para processar o formulário de inserção
    [HttpPost]
    public async Task<IActionResult> Insert(string name)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Criar e adicionar o acesso
                BookPublisher bookPublisher = new(name);
                await _unitOfWorkRepository.BookPublisherRepository.Insert(bookPublisher);
                await _unitOfWorkRepository.Save();
                return Json(new { success = true, message = "Editora adicionada com sucesso!" });
            }
        }

        // Se o nome for inválido, retorna falha
        return Json(new { success = false, message = "Erro ao adicionar Editora. Tente novamente." });
    }

    [HttpGet]
    public async Task<IActionResult> Update(short id)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        BookPublisher bookPublisher = await _unitOfWorkRepository.BookPublisherRepository.GetById(id);
        if (bookPublisher is null) return NotFound();
        return PartialView("_BookPublisherForm", bookPublisher);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short bookPublisherId, string name)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (string.IsNullOrWhiteSpace(name)) // Valida o nome
        {
            return Json(new { success = false, message = "Nome não pode ser vazio." });
        }

        // Buscar a permissão pelo ID
        var publisher = await _unitOfWorkRepository.BookPublisherRepository.GetById(bookPublisherId);
        if (publisher == null)
        {
            return Json(new { success = false, message = "Editora não encontrada." });
        }

        // Atualizar o nome da permissão
        publisher.UpdateBookPublisher(name);

        // Atualizar no banco de dados
        await _unitOfWorkRepository.BookPublisherRepository.Update(publisher);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Editora atualizada com sucesso!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        BookPublisher bookPublisher = await _unitOfWorkRepository.BookPublisherRepository.GetById(id);
        if (bookPublisher is null) return NotFound();

        await _unitOfWorkRepository.BookPublisherRepository.Delete(bookPublisher);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Editora Excluida com sucesso!" });
    }
}
