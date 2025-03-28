using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("readercomments")]
[Index("BookId", Name = "FK_ReaderComments_Books")]
[Index("UserId", Name = "FK_ReaderComments_Users")]
public partial class Readercomments
{
    /// <summary>
    /// 推荐编号
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 图书编号
    /// </summary>
    [Column("BookID", TypeName = "int(11)")]
    public int BookId { get; set; }

    /// <summary>
    /// 用户编号
    /// </summary>
    [Column("UserID", TypeName = "int(11)")]
    public int UserId { get; set; }

    /// <summary>
    /// 推荐标题
    /// </summary>
    [StringLength(100)]
    public string CommentTitle { get; set; } = null!;

    /// <summary>
    /// 推荐内容
    /// </summary>
    [Column(TypeName = "text")]
    public string Comment { get; set; } = null!;

    /// <summary>
    /// 推荐日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("Readercomments")]
    public virtual Books Book { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Readercomments")]
    public virtual Users User { get; set; } = null!;
}
