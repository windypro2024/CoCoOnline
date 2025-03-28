using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("userstates")]
public partial class Userstates
{
    /// <summary>
    /// 状态编号
    /// </summary>
    [Key]
    [Column("UserStateID", TypeName = "int(11)")]
    public int UserStateId { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    [StringLength(50)]
    public string StateName { get; set; } = null!;

    [InverseProperty("UserState")]
    public virtual ICollection<Users> Users { get; set; } = new List<Users>();
}
