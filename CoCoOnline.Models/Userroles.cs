using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("userroles")]
public partial class Userroles
{
    /// <summary>
    /// 角色编号
    /// </summary>
    [Key]
    [Column("RoleID", TypeName = "int(11)")]
    public int RoleId { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<Users> Users { get; set; } = new List<Users>();
}
