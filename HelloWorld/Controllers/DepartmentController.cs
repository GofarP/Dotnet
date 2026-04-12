using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloWorld.Data;
using HelloWorld.Models;
using System.Security.Principal;

namespace HelloWorld.Controllers;

public class DepartmentController : Controller
{
    private readonly ApplicationDbContext _context;
    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var departments = await _context.Departments.ToListAsync();
        return View(departments);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id);
        if (department == null) return NotFound();
        return View(department);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department department)
    {
        if (ModelState.IsValid)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Department '{department.Name}' berhasil ditambahkan!";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Gagal menyimpan! Pastikan semua kolom diisi dengan benar.";
        return View(department);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        return View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id);
        if (department == null) return NotFound();

        return View(department);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department != null)
        {
            _context.Departments.Remove(department);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool DepartmentExists(int id)
    {
        return _context.Departments.Any(e => e.Id == id);
    }
}