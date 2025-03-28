using CoCoOnline.Models;
using CoCoOnline.Uitles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoCoOnline.Repository
{
    public class UserstatesRepository(AppDbContext context) : BaseRepository<Userstates>(context)
    {
        public new List<Userstates> GetList()
        {
            return DbSet
                .AsNoTracking()
                .Include(u => u.Users)
                .ToList();
        }
        public new List<Userstates> GetList(Expression<Func<Userstates, bool>> predicate)
        {
            return DbSet
                .Where(predicate)   // 生成SQL语句在数据库端过滤
                .AsNoTracking()     //在只读查询中禁用跟踪
                .Include(u => u.Users)
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
        public IQueryable<Userstates> GetPagedList(int pageIndex, int pageSize, out int totalCount, Expression<Func<Userstates, bool>> predicate, Expression<Func<Userstates, object>> orderBy, bool isAscending = true)
        {
            // 参数验证
            if (pageIndex < 1) throw new ArgumentException("页码必须大于0", nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentException("每页数量必须大于0", nameof(pageSize));
            if (orderBy == null) throw new ArgumentNullException(nameof(orderBy), "排序条件不能为空");

            // 计算跳过的数据量
            int skip = (pageIndex - 1) * pageSize;

            // 获取总记录数
            totalCount = DbSet.Where(predicate).Count();

            // 构建基础查询
            IQueryable<Userstates> query = DbSet.AsNoTracking()
                .Include(u => u.Users)
                .Where(predicate);

            // 动态排序
            query = isAscending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

            // 分页查询
            IQueryable<Userstates> items = query
                .Skip(skip)
                .Take(pageSize);

            return items;
        }
        public ReturnData<Userstates?> Delete(string strIds)
        {
            ReturnData<Userstates?> returnData = new ReturnData<Userstates?>(0);
            if (strIds.Contains(','))
            {
                string[] stIds = strIds.Split(',');
                List<int> list = new List<int>();
                foreach (var id in stIds)
                {
                    list.Add(int.Parse(id));
                }
                if (list.Count > 0)
                {
                    returnData.IntResult = DbSet
                        .Where(u => list.Contains(u.UserStateId))
                        .ExecuteDelete() > 0 ? 1 : 0;
                }
                else
                {
                    returnData.ResultData = null;
                    returnData.IntResult = 0;
                }
                return returnData;
            }
            else if (string.IsNullOrEmpty(strIds))
            {
                returnData.ResultData = null;
                returnData.IntResult = 0;
                return returnData;
            }
            else
            {
                returnData.IntResult = DbSet
                    .Where(u => u.UserStateId == int.Parse(strIds))
                    .ExecuteDelete();
                return returnData;
            }
        }
    }
}
