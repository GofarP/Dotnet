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

    // GET: Daftar Claim berdasarkan Role
    public async Task<IActionResult> Index(string roleId)
    {
        if (string.IsNullOrEmpty(roleId)) return NotFound();

        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return NotFound();

        var claims = await _roleManager.GetClaimsAsync(role);
        
        ViewBag.RoleName = role.Name;
        ViewBag.RoleId = role.Id;

        return View(claims);
    }

    // POST: Tambah Claim Baru
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string roleId, string claimType, string claimValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return NotFound();

        if (!string.IsNullOrEmpty(claimType) && !string.IsNullOrEmpty(claimValue))
        {
            var result = await _roleManager.AddClaimAsync(role, new Claim(claimType, claimValue));
            if (result.Succeeded) TempData["SuccessMessage"] = "Izin berhasil ditambahkan!";
        }

        return RedirectToAction(nameof(Index), new { roleId = roleId });
    }

    // GET: Form Edit Claim
    [HttpGet]
    public async Task<IActionResult> Edit(string roleId, string claimType, string claimValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return NotFound();

        var claims = await _roleManager.GetClaimsAsync(role);
        var claim = claims.FirstOrDefault(c => c.Type == claimType && c.Value == claimValue);
        if (claim == null) return NotFound();

        ViewBag.RoleId = roleId;
        ViewBag.RoleName = role.Name;
        ViewBag.OldType = claimType;
        ViewBag.OldValue = claimValue;

        return View();
    }

    // POST: Simpan Perubahan Claim (Hapus Lama -> Tambah Baru)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string roleId, string oldType, string oldValue, string newType, string newValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return NotFound();

        var claims = await _roleManager.GetClaimsAsync(role);
        var oldClaim = claims.FirstOrDefault(c => c.Type == oldType && c.Value == oldValue);

        if (oldClaim != null)
        {
            // Identity tidak punya fungsi 'UpdateClaim', jadi harus hapus & tambah
            await _roleManager.RemoveClaimAsync(role, oldClaim);
            var result = await _roleManager.AddClaimAsync(role, new Claim(newType, newValue));

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Izin berhasil diperbarui!";
                return RedirectToAction(nameof(Index), new { roleId = roleId });
            }
        }
        return View();
    }

    // POST: Hapus Claim
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string roleId, string claimType, string claimValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return NotFound();

        var claims = await _roleManager.GetClaimsAsync(role);
        var claim = claims.FirstOrDefault(c => c.Type == claimType && c.Value == claimValue);

        if (claim != null)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
            TempData["SuccessMessage"] = "Izin berhasil dihapus!";
        }

        return RedirectToAction(nameof(Index), new { roleId = roleId });
    }
}