using System.ComponentModel.DataAnnotations;

namespace MemoryLane_D2_MVC.Models;

public class Memory
{
    public int Id { get; set; }
    [Required, StringLength(120)]
    public string Title { get; set; } = "";
    [Required, StringLength(3000)]
    public string Story { get; set; } = "";
    [DataType(DataType.Date)]
    public DateTime MemoryDate { get; set; } = DateTime.Today;
    [StringLength(60)]
    public string Category { get; set; } = "Daily Life";
    [StringLength(120)]
    public string? Place { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public string UserId { get; set; } = "";
    public ApplicationUser? User { get; set; }
}
