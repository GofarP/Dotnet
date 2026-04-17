using System.ComponentModel.DataAnnotations;

namespace HelloWorld.Models;

public class VendingMachine
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string SerialNumber { get; set; }

    [StringLength(255)]
    public string Location { get; set; }

    public int Capacity { get; set; }

    public VendingMachineStatus Status { get; set; }

    public DateTime? LastRestockDate { get; set; }

    public virtual ICollection<VendingMachineProduct> VendingMachineProducts { get; set; } = new List<VendingMachineProduct>();
}

public enum VendingMachineStatus
{
    Active = 0,
    UnderMaintenance = 1,
    OutOfOrder = 2,
    Empty = 3
}