using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloWorld.Data;
using HelloWorld.Models;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;

namespace HelloWorld.Controllers;

[Authorize]
public class DepartmentController : Controller
{
    private readonly ApplicationDbContext _context;
    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
    {
        int pageSize = 10;
        var departments = _context.Departments.AsQueryable();
        if (!string.IsNullOrEmpty(searchString))
        {
            departments = departments.Where(d =>
            d.Name.Contains(searchString) ||
            d.Description.Contains(searchString));

            ViewData["CurrentFilter"] = searchString;
        }

        int totalRecords = await departments.CountAsync();

        int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var pagedData = await departments
             .Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync();

        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = pageNumber;

        return View(pagedData);
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
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Department department)
    {
        // Pastikan ID di URL sama dengan ID dari form tersembunyi
        if (id != department.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Update data di database
                _context.Update(department);
                await _context.SaveChangesAsync();

                // Pesan sukses
                TempData["SuccessMessage"] = $"Department '{department.Name}' berhasil diperbarui!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(department.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Gagal mengubah data! Silakan periksa kembali isian form.";
        return View(department);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department != null)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Data berhasil dihapus!";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool DepartmentExists(int id)
    {
        return _context.Departments.Any(e => e.Id == id);
    }
}