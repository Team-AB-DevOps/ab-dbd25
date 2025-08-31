using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("persons")]
public class Person
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
    [Column("birth_date", TypeName = "date")]
    public DateOnly BirthDate { get; set; }

    [Required]
    [Column("gender", TypeName = "varchar(255)")]
    public string Gender { get; set; }
    
    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<MediaPersonRole> MediaPersonRoles { get; set; } = new HashSet<MediaPersonRole>();
}
