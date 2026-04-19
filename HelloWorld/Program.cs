using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HelloWorld.Data;
using HelloWorld.Models;
using HelloWorld.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' tidak ditemukan.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme="SmartScheme";
    options.DefaultChallengeScheme="SmartScheme";
    options.DefaultAuthenticateScheme="SmartScheme";
})
.AddPolicyScheme("SmartScheme", "Bearer or Cookie", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            return IdentityConstants.BearerScheme;
        }     

        return IdentityConstants.ApplicationScheme;
    };
});

builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options=>
{
    options.SignIn.RequireConfirmedAccount=false;
    options.Password.RequireDigit=false;
    options.Password.RequiredLength=6;
    options.Password.RequireNonAlphanumeric=false;
    options.Password.RequireUppercase=false; 
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath="/Identity/Account/Login";
    options.AccessDeniedPath="/Identity/Account/AccessDenied";

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }

        return Task.CompletedTask;
    };
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval=TimeSpan.FromSeconds(5); 
});

var app=builder.Build();

using(var scope = app.Services.CreateScope())
{
    var services=scope.ServiceProvider;
    try
    {
        var context=services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await DbInitializer.seed(context, userManager);
    }
    catch(Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Terjadi kesalahan saat seeding.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseStatusCodePages(async context =>
{
   if (context.HttpContext.Response.StatusCode == StatusCodes.Status401Unauthorized &&
        context.HttpContext.Request.Path.StartsWithSegments("/api"))
    {
        context.HttpContext.Response.ContentType="application/json";
        var response=new {message="Unauthorized", detail="Access Unauthorized"};
        await context.HttpContext.Response.WriteAsJsonAsync(response);
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api/auth").MapIdentityApi<ApplicationUser>(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

