using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoCoOnline.Models
{
    public class ShopCarts
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [Column("UserID", TypeName = "int(11)")]
        public int UserId { get; set; }
        /// <summary>
        /// 图书编号
        /// </summary>
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
        /// 类别编号
        /// </summary>
        [Column("CategoryID", TypeName = "int(11)")]
        public int CategoryId { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        [Precision(18, 0)]
        public decimal UnitPrice { get; set; }       
        /// <summary>
        /// 数量
        /// </summary>
        [Column(TypeName = "int(11)")]
        public int Quantity { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        [Precision(10, 2)]
        public decimal TotalPrice { get; set; }

    }
}
