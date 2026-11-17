using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MemoryLane_D2_MVC.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? FullName { get; set; }
}
