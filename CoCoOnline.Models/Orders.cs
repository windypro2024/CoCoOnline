using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("orders")]
[Index("UserId", Name = "FK_Orders_Users")]
public partial class Orders
{
    /// <summary>
    /// 订单编号
    /// </summary>
    [Key]
    [Column("OrderID", TypeName = "int(11)")]
    public int OrderId { get; set; }

    /// <summary>
    /// 订单日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 用户编号
    /// </summary>
    [Column("UserID", TypeName = "int(11)")]
    public int UserId { get; set; }

    /// <summary>
    /// 总价
    /// </summary>
    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int OrderState { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<Orderbooks> Orderbooks { get; set; } = new List<Orderbooks>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual Users User { get; set; } = null!;
}
