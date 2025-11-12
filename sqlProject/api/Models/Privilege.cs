using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace api.Models;

[Index(nameof(Name), IsUnique = true)]
[Table("privileges")]
public class Privilege
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; }

    public ICollection<User> Users { get; set; } = new HashSet<User>();

}