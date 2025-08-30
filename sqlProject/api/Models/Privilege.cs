using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace api.Models;
//id, name
[Table("privileges")]
public class Privilege
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name", TypeName = "varchar(255)")]
    [Required]
    public string Name { get; set; }

    [Column("test", TypeName = "varchar(255)")]
    public string Test { get; set; }
}