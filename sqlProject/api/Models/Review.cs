using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("reviews")]
public class Review
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [ForeignKey("profiles")]
    [Column("profile_id")]
    public int ProfileId { get; set; }

    public Profile Profile { get; set; }
    
    [Column("description", TypeName = "text")]
    public string Description { get; set; }

    [Required]
    [Column("rating", TypeName = "smallint")]
    public int Rating { get; set; }
    
    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<Media> Medias { get; set; } = new HashSet<Media>();
}