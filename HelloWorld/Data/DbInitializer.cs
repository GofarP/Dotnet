using Microsoft.AspNetCore.Identity;
using HelloWorld.Data;

public static class DbInitializer
{
    public static async Task seed(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        context.Database.EnsureCreated();

        if (context.Users.Any()) return;

        var adminUser = new IdentityUser
        {
            UserName = "admin@vending.com",
            Email = "admin@vending.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");

        await context.SaveChangesAsync();
    }
}