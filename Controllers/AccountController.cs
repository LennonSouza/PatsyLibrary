using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatsyLibrary.ViewModels;
using PatsyLibrary.Contracts.Services.Interfaces;

namespace PatsyLibrary.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl) => View(new LoginViewModel() { ReturnUrl = returnUrl });

    [HttpGet]
    public IActionResult AccessDenied() => View();

    // Processa o login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (isSuccess, message) = await _userService.LoginAsync(model.Username, model.Password);

        if (!isSuccess) return RedirectToAction("Index", "Home");

        // Chama o método SetUserSession para armazenar o nome do usuário na sessão
        _userService.SetUserSession(model.Username);

        return RedirectToAction("Index", "Home"); // Redireciona para a página principal
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}