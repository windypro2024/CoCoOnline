using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoCoOnline.Repository
{
    /// <summary>
    /// (AppDbContext context)直接写在类名后的括号中，是C#引入的‌构造函数‌语法。
    /// 作用：自动生成私有字段 context 并初始化
    /// where T : class, new() 是泛型约束
    /// class约束‌：要求 T 必须是引用类型（如类、接口、委托等）。
    /// new()约束‌：要求 T 必须有无参数的公共构造函数，确保可实例化（如 new T()）‌
    /// </summary>
    /// <typeparam name="T">是泛型类型参数，</typeparam>
    /// <param name="context"></param>
    public class BaseRepository<T>(AppDbContext context) where T : class, new()
    {
        protected DbSet<T> DbSet => context.Set<T>();
        public int Insert(T entity)
        {
            DbSet.Add(entity);
            return context.SaveChanges();
        }
        public int Delete(T entity)
        {
            DbSet.Remove(entity);
            return context.SaveChanges();
        }
        public int  Update(T entity)
        {
            DbSet.Update(entity);            
            return context.SaveChanges();
        }
        public T? Get(int id)
        {
            return DbSet.Find(id);
        }
        public List<T> GetList()
        {
            return DbSet.AsNoTracking().ToList();
        }
        /// <summary>
        /// 该方法用于从数据库表中查询符合特定条件的实体集合，支持 编译时类型安全 和 数据库端高效过滤。
        /// </summary>
        /// <param name="predicate">表示一个 表达式树,允许EF Core 解析表达式结构并生成对应的 SQL 查询条件。</param>
        /// <returns>强类型实体集合，避免装箱/拆箱操作。</returns>
        public List<T> GetList(Expression<Func<T, bool>> predicate)
        {
            return DbSet
                .Where(predicate)   // 生成SQL语句在数据库端过滤
                .AsNoTracking()     //在只读查询中禁用跟踪
                .ToList();          // 执行查询并返回结果
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="predicate">条件表达式</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="isAscending">是否升序，默认升序</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public List<T> GetPagedList(int pageIndex,int pageSize, Expression<Func<T, bool>> predicate,Expression<Func<T, object>> orderBy,bool isAscending = true)
        {
            // 参数验证
            if (pageIndex < 1) throw new ArgumentException("页码必须大于0", nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentException("每页数量必须大于0", nameof(pageSize));
            if (orderBy == null) throw new ArgumentNullException(nameof(orderBy), "排序条件不能为空");

            // 计算跳过的数据量
            int skip = (pageIndex - 1) * pageSize;

            // 获取总记录数
            int totalCount = DbSet.Count();

            // 构建基础查询
            IQueryable<T> query = DbSet.AsNoTracking().Where(predicate);

            // 动态排序
            query = isAscending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

            // 分页查询
            List<T> items = query
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return items;
        }
    }
}
