using Microsoft.AspNetCore.Mvc;
using HelloWorld.Models;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using HelloWorld.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace HelloWorld.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EmployeeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _userManager.Users
            .Include(u => u.Department)
            .ToListAsync();

            return View(employees);
        }

        public IActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string FullName, string Position, string PhoneNumber, int DepartmentId, string Email, string Password)
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
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Akun {FullName} berhasil dibuat";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.DepartmentId = new SelectList(_context.Departments, "Id", "Name", DepartmentId);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            ViewBag.DepartmentId = new SelectList(_context.Departments, "Id", "Name", user.DepartmentId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string FullName, string Position, string PhoneNumber, int DepartmentId, bool IsActive)
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
                    TempData["SuccessMessage"] = "Data karyawan berhasil diperbarui!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.DepartmentId = new SelectList(_context.Departments, "Id", "Name", DepartmentId);
            return View(user);
        }

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