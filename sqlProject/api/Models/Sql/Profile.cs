using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Sql;

[Table("profiles")]
public class Profile
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("users")]
    [Column("user_id")]
    public int UserId { get; set; }

    public User User { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; } = "Default";

    [Column("is_child", TypeName = "boolean")]
    public bool? IsChild { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public WatchList WatchList { get; set; }

    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}
