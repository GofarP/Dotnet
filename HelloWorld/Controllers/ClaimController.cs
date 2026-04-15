using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelloWorld.Controllers;

public class ClaimController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public ClaimController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    // 1. LIST (INDEX)
    public async Task<IActionResult> Index(string roleId)
    {
        if (string.IsNullOrEmpty(roleId)) return RedirectToAction("Index", "Role");

        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return NotFound();

        var claims = await _roleManager.GetClaimsAsync(role);
        
        ViewBag.RoleName = role.Name;
        ViewBag.RoleId = role.Id;

        return View(claims);
    }

    // 2. CREATE (GET)
    public IActionResult Create(string roleId)
    {
        ViewBag.RoleId = roleId;
        return View();
    }

    // CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string roleId, string claimType, string claimValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            await _roleManager.AddClaimAsync(role, new Claim(claimType, claimValue));
            TempData["SuccessMessage"] = "Izin berhasil ditambah!";
            return RedirectToAction(nameof(Index), new { roleId });
        }
        return View();
    }

    // 3. EDIT (GET)
    public async Task<IActionResult> Edit(string roleId, string type, string value)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return NotFound();

        ViewBag.RoleId = roleId;
        ViewBag.OldType = type;
        ViewBag.OldValue = value;

        return View();
    }

    // EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string roleId, string oldType, string oldValue, string newType, string newValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var claim = claims.FirstOrDefault(c => c.Type == oldType && c.Value == oldValue);

            if (claim != null)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
                await _roleManager.AddClaimAsync(role, new Claim(newType, newValue));
                TempData["SuccessMessage"] = "Izin berhasil diubah!";
            }
        }
        return RedirectToAction(nameof(Index), new { roleId });
    }

    // 4. DELETE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string roleId, string type, string value)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var claim = claims.FirstOrDefault(c => c.Type == type && c.Value == value);
            if (claim != null)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
                TempData["SuccessMessage"] = "Izin berhasil dihapus!";
            }
        }
        return RedirectToAction(nameof(Index), new { roleId });
    }
}