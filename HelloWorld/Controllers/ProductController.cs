using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.AspNetCore.Authorization;

namespace HelloWorld.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. INDEX - Tampilkan & Cari
    // [Authorize(Policy = "product.view")]
    public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
    {
        int pageSize = 10;
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(p => p.Name.Contains(searchString));
            ViewData["CurrentFilter"] = searchString;
        }

        int totalRecords = await query.CountAsync();
        var pagedData = await query
             .OrderByDescending(p => p.Id)
             .Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.CurrentPage = pageNumber;

        return View(pagedData);
    }

    // 2. CREATE (GET)
    // [Authorize(Policy = "product.create")]
    public IActionResult Create()
    {
        return View();
    }

    // 3. CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "product.create")]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Product '{product.Name}' berhasil ditambahkan!";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Gagal menyimpan! Periksa kembali inputan Anda.";
        return View(product);
    }

    // 4. EDIT (GET)
    // [Authorize(Policy = "product.edit")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        return View(product);
    }

    // 5. EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "product.edit")]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data produk berhasil diperbarui!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                else throw;
            }
        }
        return View(product);
    }

    // 6. DELETE
    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "product.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Produk berhasil dihapus!";
        }
        return RedirectToAction(nameof(Index));
    }
}