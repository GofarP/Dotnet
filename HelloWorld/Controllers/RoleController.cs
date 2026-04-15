using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloWorld.Controllers;

public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
    {
        int pageSize = 10;

        var query = _roleManager.Roles;

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(r => r.Name.Contains(searchString));
        }

        int totalRoles = await query.CountAsync();

        // 4. Ambil data dengan paging
        var roles = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.SearchString = searchString;
        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRoles / (double)pageSize);

        return View(roles);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role {roleName} berhasil dibuat!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View();
    }

    // 5. EDIT ROLE (GET)
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        return View(role);
    }

    // 6. EDIT ROLE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, string roleName)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(roleName))
        {
            role.Name = roleName.Trim();
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role berhasil diubah menjadi {roleName}!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role berhasil dihapus!";
            }
        }

        return RedirectToAction(nameof(Index));
    }
}