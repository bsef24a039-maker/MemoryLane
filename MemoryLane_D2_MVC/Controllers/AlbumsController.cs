
using MemoryLane_D2_MVC.Interfaces;
using MemoryLane_D2_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MemoryLane_D2_MVC.Controllers;

[Authorize]
public class AlbumsController : Controller
{
    private readonly IAlbumRepository _albumRepository;

    public AlbumsController(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        var albums = await _albumRepository.GetByUserIdAsync(UserId);

        return View(albums);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Album model)
    {
        ModelState.Remove("User");
        ModelState.Remove("UserId");

        if (!ModelState.IsValid)
            return View(model);

        model.UserId = UserId;

        await _albumRepository.CreateAsync(model);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _albumRepository.DeleteAsync(id, UserId);

        return RedirectToAction(nameof(Index));
    }
}
