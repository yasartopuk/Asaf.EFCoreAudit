using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asaf.AuditLog.Web.Data;
using Asaf.AuditLog.Web.Data.Entities;

namespace Asaf.AuditLog.Web.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(p => p.Category).ToListAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Product model)
    {
        if (ModelState.IsValid)
        {
            var product = await _context.Products.FindAsync(model.Id);
            product.Name = model.Name;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}