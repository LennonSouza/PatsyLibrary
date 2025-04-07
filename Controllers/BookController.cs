using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.DTOs;
using PatsyLibrary.Helpers;
using PatsyLibrary.Models;
using PatsyLibrary.ViewModels;

namespace PatsyLibrary.Controllers;

public class BookController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IBookService _bookService;

    public BookController(IUnitOfWorkRepository unitOfWorkRepository, IBookService bookService)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
        _bookService = bookService;
    }

    // Action para exibir a página inicial (index)
    public async Task<IActionResult> Index()
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        ViewBag.DepartmentList = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();
        ViewBag.BookGenderList = _unitOfWorkRepository.BookGenderRepository.GetAll.ToList();
        ViewBag.BookPublisherList = _unitOfWorkRepository.BookPublisherRepository.GetAll.ToList();

        var books = _unitOfWorkRepository.BookRepository.GetAll.ToList();
        return View(books);
    }

    // Action para exibir o formulário de inserção
    public async Task<IActionResult> Insert()
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });
        // Retorna a view parcial para o modal de adicionar
        return PartialView("_BookForm", new Book());
    }

    // INSERIR LIVRO AUTOMATICAMENTE (RECEBE JSON)
    [HttpPost]
    public async Task<IActionResult> InsertAutomatic([FromBody] InsertBookViewModel insertBook)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (string.IsNullOrWhiteSpace(insertBook.Isbn))
            return Json(new { success = false, message = "ISBN inválido!" });

        if (insertBook.DepartmentId == 0)
            return Json(new { success = false, message = "Departamento inválido!" });

        Book bookExist = await _unitOfWorkRepository.BookRepository.GetBookByISBNAsync(insertBook.Isbn, insertBook.DepartmentId);
        if (bookExist is not null)
            return Json(new { success = true, message = $"Livro {bookExist.Title} já existe!" });

        var openLibraryBookAPI = await _bookService.OpenLibrarySearchBookISBNAsync(insertBook.Isbn);
        if (openLibraryBookAPI.Book is not null)
        {
            BookPublisher editora = await _unitOfWorkRepository.BookPublisherRepository.GetByName(openLibraryBookAPI.Publisher);
            if (editora is null)
            {
                var publisher = new BookPublisher(openLibraryBookAPI.Publisher);

                await _unitOfWorkRepository.BookPublisherRepository.Insert(publisher);
                await _unitOfWorkRepository.Save();

                openLibraryBookAPI.Book.BookPublisherId = publisher.BookPublisherId;
            }
            else
            {
                openLibraryBookAPI.Book.BookPublisherId = editora.BookPublisherId;
            }

            openLibraryBookAPI.Book.DepartmentId = insertBook.DepartmentId;

            await _unitOfWorkRepository.BookRepository.Insert(openLibraryBookAPI.Book);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = $"Livro {openLibraryBookAPI.Book.Title} adicionado com sucesso!" });
        }

        var googleBookAPI = await _bookService.GoogleSearchBookISBNAsync(insertBook.Isbn);
        if (googleBookAPI.Book is not null)
        {
            string generoTranslate = BookHelper.TranslateGenre(googleBookAPI.Genre);

            BookGender genero = await _unitOfWorkRepository.BookGenderRepository.GetByName(generoTranslate);
            if (genero is null)
            {
                var gender = new BookGender(googleBookAPI.Genre);

                await _unitOfWorkRepository.BookGenderRepository.Insert(gender);
                await _unitOfWorkRepository.Save();

                googleBookAPI.Book.BookGenderId = gender.BookGenderId;
            }
            else
            {
                googleBookAPI.Book.BookGenderId = genero.BookGenderId;
            }

            BookPublisher editora = await _unitOfWorkRepository.BookPublisherRepository.GetByName(googleBookAPI.Publisher);
            if (editora is null)
            {
                var publisher = new BookPublisher(googleBookAPI.Publisher);

                await _unitOfWorkRepository.BookPublisherRepository.Insert(publisher);
                await _unitOfWorkRepository.Save();

                googleBookAPI.Book.BookPublisherId = publisher.BookPublisherId;
            }
            else
            {
                googleBookAPI.Book.BookPublisherId = editora.BookPublisherId;
            }

            googleBookAPI.Book.DepartmentId = insertBook.DepartmentId;

            await _unitOfWorkRepository.BookRepository.Insert(googleBookAPI.Book);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = $"Livro {googleBookAPI.Book.Title} adicionado com sucesso!" });
        }

        return Json(new { success = false, message = "ISBN invalido ou não encontrado, adicione manualmente!" });
    }

    // INSERIR LIVRO MANUALMENTE (RECEBE CAMPOS INDIVIDUAIS)
    [HttpPost]
    public async Task<IActionResult> InsertManual([FromBody] BookDto book)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();

            return Json(new { success = false, message = "Erro ao adicionar livro.", errors });
        }

        if (string.IsNullOrWhiteSpace(book.Author) || string.IsNullOrWhiteSpace(book.Tittle))
        {
            return Json(new { success = false, message = "Autor e título são obrigatórios." });
        }

        Book bookExist = await _unitOfWorkRepository.BookRepository.GetBookByISBNAsync(book.Isbn, book.DepartmentId);
        if (bookExist is not null)
            return Json(new { success = true, message = $"Livro {bookExist.Title} já existe!" });

        try
        {
            Book bookValidation = new(
                 book.Author,
                 book.Tittle,
                 book.PublicationYear,
                 book.AmountPage,
                 book.Sinopse,
                 book.Isbn,
                 book.Language,
                 book.DepartmentId,
                 book.BookGenderId,
                 book.BookPublisherId);

            await _unitOfWorkRepository.BookRepository.Insert(bookValidation);
            await _unitOfWorkRepository.Save();

            return Json(new { success = true, message = "Livro adicionado manualmente com sucesso!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Erro ao salvar no banco de dados: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        // Carregar listas para o dropdown
        ViewBag.BookPublisherList = _unitOfWorkRepository.BookPublisherRepository.GetAll.ToList();
        ViewBag.DepartmentList = _unitOfWorkRepository.DepartmentRepository.GetAll.ToList();
        ViewBag.BookGenderList = _unitOfWorkRepository.BookGenderRepository.GetAll.ToList();

        // Buscar o livro pelo id
        Book book = await _unitOfWorkRepository.BookRepository.GetById(id);
        if (book is null)
        {
            return Json(new { success = false, message = "Livro não encontrado!" });
        }

        return Json(new
        {
            success = true,
            book = new
            {
                book.BookId,
                book.Author,
                book.Title,
                book.PublicationYear,
                book.AmountPage,
                book.ISBN,
                book.Sinopse,
                book.Language,
                book.DepartmentId,
                book.BookGenderId,
                book.BookPublisherId
            }
        });
    }


    [HttpPost]
    public async Task<IActionResult> Update([FromBody] Book book)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        // Buscar o livro pelo ID
        var existingBook = await _unitOfWorkRepository.BookRepository.GetById(book.BookId);
        if (existingBook is null)
        {
            return Json(new { success = false, message = "Livro não encontrado." });
        }

        // Atualizar os campos do livro (se necessário)
        existingBook.Author = book.Author;
        existingBook.Title = book.Title;
        existingBook.PublicationYear = book.PublicationYear;
        existingBook.AmountPage = book.AmountPage;
        existingBook.Sinopse = book.Sinopse;
        existingBook.ISBN = book.ISBN;
        existingBook.CoverImage = book.CoverImage;
        existingBook.Language = book.Language;
        existingBook.LastUpdatedAt = DateTime.UtcNow; // Atualizando a data de modificação

        // Atualizar as relações de navegação
        existingBook.DepartmentId = book.DepartmentId;
        existingBook.BookGenderId = book.BookGenderId;
        existingBook.BookPublisherId = book.BookPublisherId;

        // Salvar as alterações no banco de dados
        await _unitOfWorkRepository.BookRepository.Update(existingBook);
        await _unitOfWorkRepository.Save();

        return Json(new { success = true, message = "Livro atualizado com sucesso!" });
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await AuthorizeHelper.AuthorizeSession(HttpContext)) return Json(new { success = false, message = "Você foi desconectado." });

        Book book = await _unitOfWorkRepository.BookRepository.GetById(id);
        if (book is null) return NotFound();

        await _unitOfWorkRepository.BookRepository.Delete(book);
        await _unitOfWorkRepository.Save();
        return Json(new { success = true, message = "Livro Excluido com sucesso!" });
    }
}