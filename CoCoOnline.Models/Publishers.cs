using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoCoOnline.Models;

[Table("publishers")]
public partial class Publishers
{
    /// <summary>
    /// 出版社编号
    /// </summary>
    [Key]
    [Column("PublisherID", TypeName = "int(11)")]
    public int PublisherId { get; set; }

    /// <summary>
    /// 出版社名称
    /// </summary>
    [StringLength(200)]
    public string PublisherName { get; set; } = null!;

    [InverseProperty("Publisher")]
    public virtual ICollection<Books> Books { get; set; } = new List<Books>();
}
