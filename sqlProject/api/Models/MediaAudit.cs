using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

[Table("medias_audit")]
public class MediaAudit
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; }

    [Required]
    [Column("action", TypeName = "varchar(255)")]
    public string Action { get; set; }

    [Required]
    [Column("date", TypeName = "timestamp with time zone")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("user", TypeName = "varchar(255)")]
    public string User { get; set; }
}
