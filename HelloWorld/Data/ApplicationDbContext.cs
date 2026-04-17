using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HelloWorld.Models;

namespace HelloWorld.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<VendingMachine> VendingMachines { get; set; }

    public DbSet<VendingMachineProduct> VendingMachineProducts{get; set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<VendingMachineProduct>()
        .HasKey(vp => new { vp.VendingMachineId, vp.ProductId });

        builder.Entity<VendingMachineProduct>()
        .HasOne(vp => vp.VendingMachine)
        .WithMany(v => v.VendingMachineProducts)
        .HasForeignKey(vp => vp.VendingMachineId);

        builder.Entity<VendingMachineProduct>()
        .HasOne(vp => vp.Product)
        .WithMany(p => p.VendingMachineProducts)
        .HasForeignKey(vp=>vp.ProductId);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Department)
            .WithMany()
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}