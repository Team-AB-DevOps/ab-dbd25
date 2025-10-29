using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

[Table("reviews")]
public class Review
{
    [ForeignKey("Media")]
    [Column("media_id")]
    public int MediaId { get; set; }
    public Media Media { get; set; }
    
    [ForeignKey("Profile")]
    [Column("profile_id")]
    public int ProfileId { get; set; }
    public Profile Profile { get; set; }
    
    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Required]
    [Column("rating", TypeName = "smallint")]
    public int Rating { get; set; }
    
    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
