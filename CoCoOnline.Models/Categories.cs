using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("categories")]
public partial class Categories
{
    /// <summary>
    /// 类别编号
    /// </summary>
    [Key]
    [Column("CategoryID", TypeName = "int(11)")]
    public int CategoryId { get; set; }

    /// <summary>
    /// 类别名称
    /// </summary>
    [StringLength(200)]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Books> Books { get; set; } = new List<Books>();
}
