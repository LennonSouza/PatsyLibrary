using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PatsyLibrary.ViewModels;

public class RegisterUserViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O email não é válido.")]
    public string Email { get; set; }

    // Sem [Required] para tornar a senha opcional na atualização
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "O departamento é obrigatório.")]
    public short DepartmentId { get; set; }

    [Required(ErrorMessage = "O cargo é obrigatório.")]
    public short RoleId { get; set; }

    public bool IsActive { get; set; }

    public IEnumerable<SelectListItem> Departments { get; set; }
    public IEnumerable<SelectListItem> Roles { get; set; }
}