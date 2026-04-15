using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HelloWorld.Data;

namespace HelloWorld.Controllers;

public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public RoleController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _context = context;
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
        var roles = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.SearchString = searchString;
        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRoles / (double)pageSize);

        return View(roles);
    }

    
    public async Task<IActionResult> Create()
    {
        // Ambil master permission untuk dropdown Select2
        ViewBag.MasterPermissions = await _context.Permissions.OrderBy(p => p.Name).ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string roleName, List<string> selectedPermissions)
    {
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var role = new IdentityRole(roleName.Trim());
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                if (selectedPermissions != null && selectedPermissions.Any())
                {
                    foreach (var permission in selectedPermissions)
                    {
                        await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    }
                }

                TempData["SuccessMessage"] = $"Role {roleName} berhasil dibuat!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        ViewBag.MasterPermissions = await _context.Permissions.OrderBy(p => p.Name).ToListAsync();
        return View();
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        ViewBag.MasterPermissions = await _context.Permissions.OrderBy(p => p.Name).ToListAsync();

        var currentClaims = await _roleManager.GetClaimsAsync(role);
        ViewBag.CurrentPermissions = currentClaims.Select(c => c.Value).ToList();

        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, string roleName, List<string> selectedPermissions)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(roleName))
        {
            role.Name = roleName.Trim();
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var oldClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in oldClaims)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }

                if (selectedPermissions != null)
                {
                    foreach (var permission in selectedPermissions)
                    {
                        await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    }
                }

                TempData["SuccessMessage"] = "Role dan izin berhasil diperbarui!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        ViewBag.MasterPermissions = await _context.Permissions.OrderBy(p => p.Name).ToListAsync();
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