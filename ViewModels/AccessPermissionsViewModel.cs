using PatsyLibrary.Models;

namespace PatsyLibrary.ViewModels;

public class AccessPermissionsViewModel
{
    public List<Permission> AvailablePermissions { get; set; }
    public List<Permission> SelectedPermissions { get; set; }
}
