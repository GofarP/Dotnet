using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace HelloWorld.Models;

public class ApplicationUser:IdentityUser
{
    [Required]
    [StringLength(100)]
    public string FullName {get; set;}=string.Empty;

    [StringLength(50)]
    public string? Position {get; set;}

    public bool? IsActive {get; set;}=true;

    public int? DepartmentId { get; set; }
    public virtual Department? Department { get; set; }
}