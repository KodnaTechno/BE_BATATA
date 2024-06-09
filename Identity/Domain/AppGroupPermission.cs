using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace AppIdentity.Domain;

public class AppGroupPermission
{
    public int Id { get; set; }
    public int AppPermissionId { get; set; }
    public AppPermission AppPermission { get; set; }
    public int GroupId { get; set; }
    public AppGroup Group { get; set; }
}