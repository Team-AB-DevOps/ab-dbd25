using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Sql;

[Table("medias_persons_roles")]
public class MediaPersonRole
{
    [ForeignKey("Media")]
    [Column("media_id")]
    public int MediaId { get; set; }
    public Media Media { get; set; }

    [ForeignKey("Person")]
    [Column("person_id")]
    public int PersonId { get; set; }
    public Person Person { get; set; }

    [ForeignKey("Role")]
    [Column("role_id")]
    public int RoleId { get; set; }
    public Role Role { get; set; }
}
