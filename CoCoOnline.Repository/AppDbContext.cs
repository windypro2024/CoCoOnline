using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CoCoOnline.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CoCoOnline.Repository
{
    public partial class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {            
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion);
        }

        public virtual DbSet<Books> Books { get; set; }

        public virtual DbSet<Categories> Categories { get; set; }

        public virtual DbSet<Orderbooks> Orderbooks { get; set; }

        public virtual DbSet<Orders> Orders { get; set; }

        public virtual DbSet<Publishers> Publishers { get; set; }

        public virtual DbSet<Readercomments> Readercomments { get; set; }

        public virtual DbSet<Searchkeywords> Searchkeywords { get; set; }

        public virtual DbSet<Userroles> Userroles { get; set; }

        public virtual DbSet<Users> Users { get; set; }

        public virtual DbSet<Userstates> Userstates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Books>(entity =>
            {
                entity.HasKey(e => e.BookId).HasName("PRIMARY");

                entity.Property(e => e.BookId).HasComment("图书编号");
                entity.Property(e => e.AuthorDescription).HasComment("作者简介");
                entity.Property(e => e.BookAuthor).HasComment("作者");
                entity.Property(e => e.BookTitle).HasComment("图书名称");
                entity.Property(e => e.CategoryId).HasComment("类别编号");
                entity.Property(e => e.Clicks)
                    .HasDefaultValueSql("'1'")
                    .HasComment("点击次数");
                entity.Property(e => e.ContentDescription).HasComment("内容摘要");
                entity.Property(e => e.DeleteFlag).HasComment("删除");
                entity.Property(e => e.EditorComment).HasComment("编辑推荐");
                entity.Property(e => e.ImgUrl).HasComment("封面");
                entity.Property(e => e.Isbn).HasComment("ISBN");
                entity.Property(e => e.PublishDate).HasComment("出版社日期");
                entity.Property(e => e.PublisherId).HasComment("出版社编号");
                entity.Property(e => e.Toc).HasComment("目录");
                entity.Property(e => e.UnitPrice).HasComment("单价");
                entity.Property(e => e.WordsCount).HasComment("字数");

                entity.HasOne(d => d.Category).WithMany(p => p.Books)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Books_Categories");

                entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Books_Publishers");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

                entity.Property(e => e.CategoryId).HasComment("类别编号");
                entity.Property(e => e.CategoryName).HasComment("类别名称");
            });

            modelBuilder.Entity<Orderbooks>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasComment("图书订单编号");
                entity.Property(e => e.BookId).HasComment("图书编号");
                entity.Property(e => e.OrderId).HasComment("订单编号");
                entity.Property(e => e.Quantity).HasComment("数量");
                entity.Property(e => e.UnitPrice).HasComment("单价");

                entity.HasOne(d => d.Book).WithMany(p => p.Orderbooks)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderBooks_Books");

                entity.HasOne(d => d.Order).WithMany(p => p.Orderbooks)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderBooks_Orders");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PRIMARY");

                entity.Property(e => e.OrderId).HasComment("订单编号");
                entity.Property(e => e.OrderDate).HasComment("订单日期");
                entity.Property(e => e.OrderState)
                    .HasDefaultValueSql("'1'")
                    .HasComment("订单状态");
                entity.Property(e => e.TotalPrice).HasComment("总价");
                entity.Property(e => e.UserId).HasComment("用户编号");

                entity.HasOne(d => d.User).WithMany(p => p.Orders)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Users");
            });

            modelBuilder.Entity<Publishers>(entity =>
            {
                entity.HasKey(e => e.PublisherId).HasName("PRIMARY");

                entity.Property(e => e.PublisherId).HasComment("出版社编号");
                entity.Property(e => e.PublisherName).HasComment("出版社名称");
            });

            modelBuilder.Entity<Readercomments>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasComment("推荐编号");
                entity.Property(e => e.BookId).HasComment("图书编号");
                entity.Property(e => e.Comment).HasComment("推荐内容");
                entity.Property(e => e.CommentTitle).HasComment("推荐标题");
                entity.Property(e => e.Date).HasComment("推荐日期");
                entity.Property(e => e.UserId).HasComment("用户编号");

                entity.HasOne(d => d.Book).WithMany(p => p.Readercomments)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReaderComments_Books");

                entity.HasOne(d => d.User).WithMany(p => p.Readercomments)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReaderComments_Users");
            });

            modelBuilder.Entity<Searchkeywords>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasComment("搜索编号");
                entity.Property(e => e.Keyword).HasComment("搜索关键字");
                entity.Property(e => e.SearchCount).HasComment("搜索次数");
            });

            modelBuilder.Entity<Userroles>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PRIMARY");

                entity.Property(e => e.RoleId).HasComment("角色编号");
                entity.Property(e => e.RoleName).HasComment("角色名称");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PRIMARY");

                entity.Property(e => e.UserId).HasComment("用户编号");
                entity.Property(e => e.Address).HasComment("地址");
                entity.Property(e => e.Email).HasComment("EMail");
                entity.Property(e => e.LoginName).HasComment("登录名");
                entity.Property(e => e.Phone).HasComment("电话");
                entity.Property(e => e.RegDate).HasComment("注册日期");
                entity.Property(e => e.RoleId).HasComment("角色编号");
                entity.Property(e => e.UserName).HasComment("用户名");
                entity.Property(e => e.UserPwd).HasComment("密码");
                entity.Property(e => e.UserStateId).HasComment("状态编号");

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Roles");

                entity.HasOne(d => d.UserState).WithMany(p => p.Users)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_UserStates");
            });

            modelBuilder.Entity<Userstates>(entity =>
            {
                entity.HasKey(e => e.UserStateId).HasName("PRIMARY");

                entity.Property(e => e.UserStateId).HasComment("状态编号");
                entity.Property(e => e.StateName).HasComment("状态名称");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
