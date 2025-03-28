using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("searchkeywords")]
public partial class Searchkeywords
{
    /// <summary>
    /// 搜索编号
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 搜索关键字
    /// </summary>
    [StringLength(50)]
    public string Keyword { get; set; } = null!;

    /// <summary>
    /// 搜索次数
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int SearchCount { get; set; }
}
