using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("books")]
[Index("CategoryId", Name = "FK_Books_Categories")]
[Index("PublisherId", Name = "FK_Books_Publishers")]
public partial class Books
{
    /// <summary>
    /// 图书编号
    /// </summary>
    [Key]
    [Column("BookID", TypeName = "int(11)")]
    public int BookId { get; set; }

    /// <summary>
    /// 图书名称
    /// </summary>
    [StringLength(200)]
    public string BookTitle { get; set; } = null!;

    /// <summary>
    /// 作者
    /// </summary>
    [StringLength(200)]
    public string BookAuthor { get; set; } = null!;

    /// <summary>
    /// 出版社编号
    /// </summary>
    [Column("PublisherID", TypeName = "int(11)")]
    public int PublisherId { get; set; }

    /// <summary>
    /// 出版社日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime PublishDate { get; set; }

    /// <summary>
    /// ISBN
    /// </summary>
    [Column("ISBN")]
    [StringLength(50)]
    public string Isbn { get; set; } = null!;

    /// <summary>
    /// 字数
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int WordsCount { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [Precision(19, 4)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 内容摘要
    /// </summary>
    public string? ContentDescription { get; set; }

    /// <summary>
    /// 作者简介
    /// </summary>
    public string? AuthorDescription { get; set; }

    /// <summary>
    /// 编辑推荐
    /// </summary>
    public string? EditorComment { get; set; }

    /// <summary>
    /// 目录
    /// </summary>
    [Column("TOC")]
    public string? Toc { get; set; }

    /// <summary>
    /// 类别编号
    /// </summary>
    [Column("CategoryID", TypeName = "int(11)")]
    public int CategoryId { get; set; }

    /// <summary>
    /// 点击次数
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int Clicks { get; set; }

    /// <summary>
    /// 封面
    /// </summary>
    [StringLength(200)]
    public string? ImgUrl { get; set; }

    /// <summary>
    /// 删除
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int DeleteFlag { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Books")]
    public virtual Categories Category { get; set; } = null!;

    [InverseProperty("Book")]
    public virtual ICollection<Orderbooks> Orderbooks { get; set; } = new List<Orderbooks>();

    [ForeignKey("PublisherId")]
    [InverseProperty("Books")]
    public virtual Publishers Publisher { get; set; } = null!;

    [InverseProperty("Book")]
    public virtual ICollection<Readercomments> Readercomments { get; set; } = new List<Readercomments>();
}
