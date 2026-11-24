
using System.ComponentModel.DataAnnotations;

namespace MemoryLane_D2_MVC.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [StringLength(300)]
        public string? Description { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        public ApplicationUser? User { get; set; }

        public ICollection<Memory> Memories { get; set; } = new List<Memory>();
    }
}
