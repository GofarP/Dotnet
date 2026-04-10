using Microsoft.AspNetCore.Identity;
using HelloWorld.Data;
using HelloWorld.Models;

public static class DbInitializer
{
    public static async Task seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        context.Database.EnsureCreated();

        if (context.Users.Any()) return;

        var adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@vending.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");

        await context.SaveChangesAsync();
    }
}