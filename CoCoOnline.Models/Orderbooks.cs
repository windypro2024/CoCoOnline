using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("orderbooks")]
[Index("BookId", Name = "FK_OrderBooks_Books")]
[Index("OrderId", Name = "FK_OrderBooks_Orders")]
public partial class Orderbooks
{
    /// <summary>
    /// 图书订单编号
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    [Column("OrderID", TypeName = "int(11)")]
    public int OrderId { get; set; }

    /// <summary>
    /// 图书编号
    /// </summary>
    [Column("BookID", TypeName = "int(11)")]
    public int BookId { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int Quantity { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [Precision(18, 0)]
    public decimal UnitPrice { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("Orderbooks")]
    public virtual Books Book { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("Orderbooks")]
    public virtual Orders Order { get; set; } = null!;
}
