using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HelloWorld.Models;

[Table("Permissions")]
public class Permission
{
    [Key]
    public int Id {get; set;}
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [StringLength(255)]
    public string? Description { get; set; }
}