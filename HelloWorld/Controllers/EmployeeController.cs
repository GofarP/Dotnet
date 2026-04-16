using Microsoft.AspNetCore.Mvc;
using HelloWorld.Models;
using HelloWorld.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelloWorld.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public EmployeeController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // 1. LIST KARYAWAN
        public async Task<IActionResult> Index()
        {
            var employees = await _userManager.Users
                .Include(u => u.Department)
                .ToListAsync();

            return View(employees);
        }

        // 2. TAMBAH KARYAWAN (GET)
        public async Task<IActionResult> Create()
        {
            // Ambil master roles untuk Select2
            ViewBag.Roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            ViewBag.DepartmentId = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // 3. TAMBAH KARYAWAN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string FullName, string Position, string PhoneNumber, int DepartmentId, string Email, string Password, List<string> selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Email,
                    Email = Email,
                    FullName = FullName,
                    Position = Position,
                    DepartmentId = DepartmentId,
                    PhoneNumber = PhoneNumber,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, Password);
                
                if (result.Succeeded)
                {
                    // ATTACH ROLES: Tambahkan role yang dipilih dari Select2
                    if (selectedRoles != null && selectedRoles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, selectedRoles);
                    }

                    TempData["SuccessMessage"] = $"Akun {FullName} berhasil dibuat dengan role terkait.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Jika gagal, kembalikan data ke dropdown agar tidak kosong
            ViewBag.Roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            ViewBag.DepartmentId = new SelectList(_context.Departments, "Id", "Name", DepartmentId);
            return View();
        }

        // 4. EDIT KARYAWAN (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Ambil semua role yang ada di sistem
            ViewBag.Roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            
            // Ambil role yang saat ini dimiliki user (untuk pre-select di Select2)
            ViewBag.CurrentRoles = await _userManager.GetRolesAsync(user);

            ViewBag.DepartmentId = new SelectList(_context.Departments, "Id", "Name", user.DepartmentId);
            return View(user);
        }

        // 5. EDIT KARYAWAN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string FullName, string Position, string PhoneNumber, int DepartmentId, bool IsActive, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                user.FullName = FullName;
                user.Position = Position;
                user.DepartmentId = DepartmentId;
                user.PhoneNumber = PhoneNumber;
                user.IsActive = IsActive;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // SYNC ROLES: Reset role lama dan masukkan yang baru dipilih
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    
                    // Hapus role lama
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    }

                    // Tambahkan role baru dari Select2
                    if (selectedRoles != null && selectedRoles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, selectedRoles);
                    }

                    TempData["SuccessMessage"] = "Data karyawan dan hak akses berhasil diperbarui!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            ViewBag.CurrentRoles = await _userManager.GetRolesAsync(user);
            ViewBag.DepartmentId = new SelectList(_context.Departments, "Id", "Name", DepartmentId);
            return View(user);
        }

        // 6. DELETE KARYAWAN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Karyawan berhasil dihapus!";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}