using System.ComponentModel.DataAnnotations;

namespace HelloWorld.Models;
public class Department
{
    public int Id {get; set;}
    [StringLength(100)]
    public string Name {get; set;}
    [StringLength(100)]
    public string Description {get; set;}
}
