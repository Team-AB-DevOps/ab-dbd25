using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

[Index(nameof(Name), IsUnique = true)]
[Table("genres")]
public class Genre
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; }

    public ICollection<Media> Medias { get; set; } = new HashSet<Media>();

    public ICollection<Subscription> Subscriptions { get; set; } = new HashSet<Subscription>();
}