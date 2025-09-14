using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

[Index(nameof(Name), IsUnique = true)]
[Table("subscriptions")]
public class Subscription
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; }
    
    [Required]
    [Column("price", TypeName = "smallint")]
    public int Price { get; set; }

    public ICollection<User> Users { get; set; } = new HashSet<User>();

    public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
}