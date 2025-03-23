using PatsyLibrary.Models;
using PatsyLibrary.ViewModels;

namespace PatsyLibrary.Contracts.Services.Interfaces;

public interface IUserService
{
    public Task<(bool IsSuccess, string Message)> RegisterUserAsync(RegisterUserViewModel model);
    public Task<(bool IsSuccess, string Message)> LoginAsync(string userName, string password);
    public void SetUserSession(string username);
    public string GetUserSession();
}