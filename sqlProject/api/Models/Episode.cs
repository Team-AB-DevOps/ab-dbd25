using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("episodes")]
public class Episode
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [ForeignKey("medias")]
    [Column("media_id")]
    public int MediaId { get; set; }

    public Media Media { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; }
    
    [Column("season_count", TypeName = "smallint")]
    public int? SeasonCount { get; set; }

    [Required]
    [Column("episode_count", TypeName = "smallint")]
    public int EpisodeCount { get; set; }

    [Required]
    [Column("runtime", TypeName = "integer")]
    public int Runtime { get; set; }

    [Required]
    [Column("description", TypeName = "text")]
    public string Description { get; set; }
    
    [Required]
    [Column("release", TypeName = "date")]
    public DateOnly Release { get; set; }
    
    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}