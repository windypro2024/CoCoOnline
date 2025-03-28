using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("users")]
[Index("RoleId", Name = "FK_Users_Roles")]
[Index("UserStateId", Name = "FK_Users_UserStates")]
public partial class Users
{
    /// <summary>
    /// 用户编号
    /// </summary>
    [Key]
    [Column("UserID", TypeName = "int(11)")]
    public int UserId { get; set; }

    /// <summary>
    /// 登录名
    /// </summary>
    [StringLength(50)]
    public string LoginName { get; set; } = null!;

    /// <summary>
    /// 用户名
    /// </summary>
    [StringLength(50)]
    public string UserName { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    [StringLength(50)]
    public string UserPwd { get; set; } = null!;

    /// <summary>
    /// 地址
    /// </summary>
    [StringLength(200)]
    public string Address { get; set; } = null!;

    /// <summary>
    /// 电话
    /// </summary>
    [StringLength(100)]
    public string Phone { get; set; } = null!;

    /// <summary>
    /// EMail
    /// </summary>
    [Column("EMail")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    /// <summary>
    /// 角色编号
    /// </summary>
    [Column("RoleID", TypeName = "int(11)")]
    public int RoleId { get; set; }

    /// <summary>
    /// 状态编号
    /// </summary>
    [Column("UserStateID", TypeName = "int(11)")]
    public int UserStateId { get; set; }

    /// <summary>
    /// 注册日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime RegDate { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

    [InverseProperty("User")]
    public virtual ICollection<Readercomments> Readercomments { get; set; } = new List<Readercomments>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Userroles Role { get; set; } = null!;

    [ForeignKey("UserStateId")]
    [InverseProperty("Users")]
    public virtual Userstates UserState { get; set; } = null!;
}
