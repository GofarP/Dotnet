using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HelloWorld.Data;
using HelloWorld.Models; // Pastikan folder 'Data' kamu berisi ApplicationDbContext.cs

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' tidak ditemukan.");

// Registrasi DbContext menggunakan MySQL (Sesuai kode yang kamu minta)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. SETUP IDENTITY (Login & Register)
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Set false agar tidak perlu verifikasi email saat dev
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// 3. TAMBAHKAN LAYANAN MVC & RAZOR PAGES
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Wajib ada untuk UI Identity bawaan

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager=services.GetRequiredService<UserManager<ApplicationUser>>();

        await DbInitializer.seed(context, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Terjadi kesalahan saat seeding database.");
    }
}

// 4. KONFIGURASI HTTP PIPELINE (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Agar file CSS hasil build Tailwind (app.css) bisa diakses
app.UseStaticFiles();

app.UseRouting();

// Wajib berurutan: Authentication dulu baru Authorization
app.UseAuthentication();
app.UseAuthorization();

// 5. ATUR ROUTING
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Menghubungkan halaman Login/Register Identity

app.Run();