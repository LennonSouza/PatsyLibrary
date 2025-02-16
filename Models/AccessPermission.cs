namespace PatsyLibrary.Models;

public class AccessPermission
{
    public byte AccessId { get; set; }
    public Access Access { get; set; }

    public short PermissionId { get; set; }
    public Permission Permission { get; set; }
}
