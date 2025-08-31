using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("medias")]
public class Media
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; }
    
    [Required]
    [Column("type", TypeName = "varchar(255)")]
    public string Type { get; set; }

    [Column("runtime", TypeName = "integer")]
    public int Runtime { get; set; }

    [Required]
    [Column("description", TypeName = "text")]
    public string Description { get; set; }
    
    [Required] 
    [Column("cover", TypeName = "text")]
    public string Cover { get; set; }

    public int? AgeLimit { get; set; }

    [Required]
    [Column("release", TypeName = "date")]
    public DateOnly Release { get; set; }
    
    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<WatchList> WatchLists { get; set; } = new HashSet<WatchList>();
    
    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    
    public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();

    public ICollection<Episode> Episodes { get; set; } = new HashSet<Episode>();
    
    public ICollection<MediaPersonRole> MediaPersonRoles { get; set; } = new HashSet<MediaPersonRole>();
}


