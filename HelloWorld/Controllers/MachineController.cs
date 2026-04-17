using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloWorld.Data;
using HelloWorld.Models;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;

namespace HelloWorld.Controllers;

[Authorize]

public class MachineController : Controller
{
    private readonly ApplicationDbContext _context;

    public MachineController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
    {
        int pageSize = 10;
        ViewData["CurrentFilter"] = searchString;

        var query = _context.VendingMachines.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(v => v.Name.Contains(searchString) ||
                v.SerialNumber.Contains(searchString)
            );
        }

        int totalRecords = await query.CountAsync();
        var pagedData = await query
        .OrderByDescending(v => v.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        return View(pagedData);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VendingMachine vendingMachine)
    {
        if (ModelState.IsValid)
        {
            _context.Add(vendingMachine);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Vending Machine berhasil ditambahkan!";
            return RedirectToAction(nameof(Index));
        }

        return View(vendingMachine);

    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var vendingMachine = await _context.VendingMachines.FindAsync(id);
        if (vendingMachine == null) return NotFound();

        return View(vendingMachine);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VendingMachine vendingMachine)
    {
        if (id != vendingMachine.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vendingMachine);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data mesin berhasil diperbarui!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.VendingMachines.Any(e => e.Id == vendingMachine.Id)) return NotFound();
                else throw;
            }
        }
        return View(vendingMachine);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var vendingMachine = await _context.VendingMachines.FindAsync(id);
        if (vendingMachine != null)
        {
            _context.VendingMachines.Remove(vendingMachine);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Vending Machine Berhasil Dihapus";
        }

        return RedirectToAction(nameof(Index));
    }
}