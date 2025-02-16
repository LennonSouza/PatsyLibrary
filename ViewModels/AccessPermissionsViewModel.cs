using PatsyLibrary.Models;

namespace PatsyLibrary.ViewModels;

public class AccessPermissionsViewModel
{
    public Access Access { get; set; }
    public List<Permission> AvailablePermissions { get; set; }
    public List<Permission> SelectedPermissions { get; set; }
}
