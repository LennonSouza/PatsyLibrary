using System.ComponentModel.DataAnnotations;

namespace PatsyLibrary.ViewModels;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
}