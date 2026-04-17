using System.ComponentModel.DataAnnotations;

namespace HelloWorld.Models;
public class VendingMachineProduct
{
    [Key]
    public int id {get;set;}

    public int VendingMachineId{get;set;}

    public virtual VendingMachine VendingMachine{get; set;}=null!;

    public int ProductId {get; set;}

    public virtual Product Product {get; set;}=null!;

    public int Quantity {get; set;}

}