using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloWorld.Data;
using HelloWorld.Models;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Elfie.Serialization;
namespace HelloWorld.Controllers;

[Authorize]
public class PermissionController : Controller
{
    private readonly ApplicationDbContext _context;
    public PermissionController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
    {
        int pageSize = 10;
        var permissions = _context.Permissions.AsQueryable();
        if (!string.IsNullOrEmpty(searchString))
        {
            permissions = permissions.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            ViewData["CurrentFilter"] = searchString;
        }

        int totalRecords = await permissions.CountAsync();

        int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var pagedData = await permissions.Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = pageNumber;

        return View(pagedData);

    }


    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var permission = await _context.Permissions.FirstOrDefaultAsync(m => m.Id == id);
        if (permission == null) return NotFound();

        return View(permission);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Create(Permission permission)
    {
        if (ModelState.IsValid)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Department '{permission.Name}' berhasil ditambahkan!";
            return RedirectToAction(nameof(Index));

        }

        TempData["ErrorMessage"] = "Gagal menyimpan! pastikan semua kolom diisi dengan benar.";
        return View(permission);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var permission = await _context.Permissions.FindAsync(id);

        if (permission == null) return NotFound();

        return View(permission);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Permission permission)
    {
        if (id != permission.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(permission);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Permission '{permission.Name}' berhasil diperbarui!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermissionExists(permission.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }
        return RedirectToAction(nameof(Index));

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission != null)
        {
            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Data berhasil dihapus!";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool PermissionExists(int id)
    {
        return _context.Permissions.Any(e => e.Id == id);
    }

}