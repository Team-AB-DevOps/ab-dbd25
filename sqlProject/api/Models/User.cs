using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("first_name", TypeName = "varchar(255)")]
    public string FirstName { get; set; }
    
    [Required]
    [Column("last_name", TypeName = "varchar(255)")]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    [Column("email", TypeName = "varchar(255)")]
    public string Email { get; set; }
    
    [Required]
    [Column("password", TypeName = "text")]
    public string Name { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<Privilege> Privileges { get; set; } = new HashSet<Privilege>();

    public ICollection<Subscription> Subscriptions { get; set; } = new HashSet<Subscription>();

    public ICollection<Profile> Profiles { get; set; } = new HashSet<Profile>();
}