
using MemoryLane_D2_MVC.Interfaces;
using MemoryLane_D2_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MemoryLane_D2_MVC.Controllers;

[Authorize]
public class MemoriesController : Controller
{
    private readonly IMemoryRepository _memoryRepository;
    private readonly IWebHostEnvironment _env;

    public MemoriesController(
        IMemoryRepository memoryRepository,
        IWebHostEnvironment env)
    {
        _memoryRepository = memoryRepository;
        _env = env;
    }

    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Display all memories
    public async Task<IActionResult> Index(
        string? search, string? category)
    {
        ViewBag.Search = search;
        ViewBag.Category = category;

        ViewBag.Categories =
            await _memoryRepository.GetCategoriesByUserIdAsync(UserId);

        var memories = await _memoryRepository.GetByUserIdAsync(
            UserId, search, category);

        return View(memories);
    }

    // AJAX Search and Category Filter
    [HttpGet]
    public async Task<IActionResult> SearchAjax(
        string? search, string? category)
    {
        var memories = await _memoryRepository.GetByUserIdAsync(
            UserId, search, category);

        return PartialView("_MemoryList", memories);
    }

    // Display Create Form
    public IActionResult Create()
    {
        return View(new Memory { MemoryDate = DateTime.Today });
    }

    // Create Memory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Memory model, IFormFile? photo)
    {
        ModelState.Remove("User");
        ModelState.Remove("UserId");

        if (!ModelState.IsValid)
            return View(model);

        model.UserId = UserId;

        if (photo != null)
            model.ImagePath = await SavePhoto(photo);

        await _memoryRepository.CreateAsync(model);

        return RedirectToAction(nameof(Index));
    }

    // Display Edit Form
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _memoryRepository.GetByIdAsync(id, UserId);

        return item == null ? NotFound() : View(item);
    }

    // Update Memory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, Memory model, IFormFile? photo)
    {
        var item = await _memoryRepository.GetByIdAsync(id, UserId);

        if (item == null)
            return NotFound();

        ModelState.Remove("User");
        ModelState.Remove("UserId");

        if (!ModelState.IsValid)
            return View(model);

        item.Title = model.Title;
        item.Story = model.Story;
        item.MemoryDate = model.MemoryDate;
        item.Category = model.Category;
        item.Place = model.Place;

        if (photo != null)
            item.ImagePath = await SavePhoto(photo);

        await _memoryRepository.UpdateAsync(item);

        return RedirectToAction(nameof(Index));
    }

    // Display Delete Confirmation
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _memoryRepository.GetByIdAsync(id, UserId);

        return item == null ? NotFound() : View(item);
    }

    // Delete Memory
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _memoryRepository.DeleteAsync(id, UserId);

        return RedirectToAction(nameof(Index));
    }

    // Save Uploaded Photo
    private async Task<string> SavePhoto(IFormFile photo)
    {
        var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();

        if (photo.Length > 5 * 1024 * 1024 ||
            !new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
        {
            throw new InvalidOperationException(
                "Only JPG, PNG or WEBP images up to 5 MB are allowed.");
        }

        var name = $"{Guid.NewGuid()}{ext}";

        var folder = Path.Combine(_env.WebRootPath, "uploads");

        Directory.CreateDirectory(folder);

        using var stream = System.IO.File.Create(
            Path.Combine(folder, name));

        await photo.CopyToAsync(stream);

        return "/uploads/" + name;
    }
}
