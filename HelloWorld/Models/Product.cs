using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloWorld.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Range(0, 500)]
    public int Stock { get; set; }

    public int VendingMachineId { get; set; }

    [ForeignKey("VendingMachineId")]
    public virtual VendingMachine VendingMachine {get;set;}
}