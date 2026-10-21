using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIHIFA.Models;

[Table("MediaItems")]
public class MediaItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Judul { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string TipeMedia { get; set; } = MediaConstants.Types.Anime;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = MediaConstants.Statuses.PlanTo;

    [Range(0.0, 10.0)]
    public double Rating { get; set; } = 8.0;

    [MaxLength(150)]
    public string? Genre { get; set; }

    [MaxLength(500)]
    public string? PathCoverLocal { get; set; }

    [MaxLength(2000)]
    public string? Catatan { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
