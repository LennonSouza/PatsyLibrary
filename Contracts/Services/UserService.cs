using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Models;
using PatsyLibrary.ViewModels;
using static PatsyLibrary.Services.LibraryHelper;

namespace PatsyLibrary.Contracts.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IUnitOfWorkRepository unitOfWorkRepository, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWorkRepository = unitOfWorkRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool IsSuccess, string Message)> RegisterUserAsync(RegisterUserViewModel model)
    {
        // Verifica se as senhas correspondem
        if (model.Password != model.ConfirmPassword) return Result.Success(false, "As senhas não coincidem.");

        User userExist = await _unitOfWorkRepository.UserRepository.GetbyEmail(model.Email);
        if (userExist is not null) return Result.Success(false, "Email já cadastrado.");

        // Criação do usuário
        User user = new(model.UserName.ToLower(), model.Password, model.Email.ToLower(), model.DepartmentId);
        if (model.IsActive)
            user.Activate();
        else
            user.Deactivate();

        user.SetRole(model.RoleId);

        try
        {
            await _unitOfWorkRepository.UserRepository.Insert(user);
            await _unitOfWorkRepository.Save();

            return Result.Success(true, "Usuario cadastrado com sucesso.");
        }
        catch (Exception e)
        {
            return Result.Success(false, $"Erro ao registrar o usuário: {e.InnerException.Message ?? e.Message}");
        }
    }

    public async Task<(bool IsSuccess, string Message)> LoginAsync(string userName, string password)
    {
        User user = await _unitOfWorkRepository.UserRepository.GetbyUserName(userName);
        if (user is null || user.IsActive is false) return (false, "Usuário não encontrado.");

        if (user.PassWord != password) return (false, "Credenciais inválidas.");

        return (true, "Login realizado com sucesso.");
    }

    public void SetUserSession(string username)
    {
        ISession session = _httpContextAccessor.HttpContext.Session;
        session.SetString("UserName", username);
    }

    public string GetUserSession() => _httpContextAccessor.HttpContext.Session.GetString("UserName");
}