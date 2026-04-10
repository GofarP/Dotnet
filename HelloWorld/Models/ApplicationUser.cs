using Microsoft.AspNetCore.Identity;
namespace HelloWorld.Models;

public class ApplicationUser : IdentityUser
{
    public string? fullName { get; set; }
    public bool? isActive { get; set; }

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }

}
