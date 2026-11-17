using MemoryLane_D2_MVC.Data;
using MemoryLane_D2_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MemoryLane_D2_MVC.Controllers;

[Authorize]
public class MemoriesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    public MemoriesController(ApplicationDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index(string? search, string? category)
    {
        var q = _db.Memories.Where(m => m.UserId == UserId);
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(m => m.Title.Contains(search) || m.Story.Contains(search));
        if (!string.IsNullOrWhiteSpace(category)) q = q.Where(m => m.Category == category);
        ViewBag.Search = search; ViewBag.Category = category;
        ViewBag.Categories = await _db.Memories.Where(m => m.UserId == UserId).Select(m => m.Category).Distinct().ToListAsync();
        return View(await q.OrderByDescending(m => m.MemoryDate).ToListAsync());
    }
    public IActionResult Create() => View(new Memory { MemoryDate = DateTime.Today });
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Memory model, IFormFile? photo)
    {
        ModelState.Remove("User"); ModelState.Remove("UserId");
        if (!ModelState.IsValid) return View(model);
        model.UserId = UserId;
        if (photo != null) model.ImagePath = await SavePhoto(photo);
        _db.Memories.Add(model); await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.Memories.FirstOrDefaultAsync(m => m.Id == id && m.UserId == UserId);
        return item == null ? NotFound() : View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Memory model, IFormFile? photo)
    {
        var item = await _db.Memories.FirstOrDefaultAsync(m => m.Id == id && m.UserId == UserId);
        if (item == null) return NotFound();
        ModelState.Remove("User"); ModelState.Remove("UserId");
        if (!ModelState.IsValid) return View(model);
        item.Title = model.Title; item.Story = model.Story; item.MemoryDate = model.MemoryDate;
        item.Category = model.Category; item.Place = model.Place;
        if (photo != null) item.ImagePath = await SavePhoto(photo);
        await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Memories.FirstOrDefaultAsync(m => m.Id == id && m.UserId == UserId);
        return item == null ? NotFound() : View(item);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Memories.FirstOrDefaultAsync(m => m.Id == id && m.UserId == UserId);
        if (item != null) { _db.Memories.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
    private async Task<string> SavePhoto(IFormFile photo)
    {
        var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
        if (photo.Length > 5 * 1024 * 1024 || !new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
            throw new InvalidOperationException("Only JPG, PNG or WEBP images up to 5 MB are allowed.");
        var name = $"{Guid.NewGuid()}{ext}";
        var folder = Path.Combine(_env.WebRootPath, "uploads"); Directory.CreateDirectory(folder);
        using var stream = System.IO.File.Create(Path.Combine(folder, name));
        await photo.CopyToAsync(stream);
        return "/uploads/" + name;
    }
}
