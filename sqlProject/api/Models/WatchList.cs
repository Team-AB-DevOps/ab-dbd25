using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("watch_lists")]
public class WatchList
{
    [Key]
    [ForeignKey("profiles")]
    [Column("profile_id")]
    public int ProfileId { get; set; }

    public Profile Profile { get; set; }

    [Column("is_locked", TypeName = "boolean")]
    public bool? IsLocked { get; set; } = false;
    
    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Media> Medias { get; set; } = new HashSet<Media>();
}