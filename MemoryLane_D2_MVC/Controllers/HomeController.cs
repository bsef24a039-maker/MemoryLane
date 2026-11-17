using MemoryLane_D2_MVC.Data;
using MemoryLane_D2_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MemoryLane_D2_MVC.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db)
    { _db = db;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Recent = await _db.Memories.OrderByDescending(m => m.CreatedAt).Take(3).ToListAsync();
        return View();
    }
    public IActionResult Error() => View();
}
