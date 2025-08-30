using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("profiles")]
public class Profile
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; } = "Default";

    [Column("is_child", TypeName = "boolean")]
    public bool IsChild { get; set; }

    [ForeignKey("user")]
    [Column("user_id")]
    public int UserId { get; set; }

    public User User { get; set; }
}