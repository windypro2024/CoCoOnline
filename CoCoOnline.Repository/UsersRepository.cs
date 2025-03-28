using CoCoOnline.Models;
using CoCoOnline.Uitles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CoCoOnline.Repository
{
    public class UsersRepository(AppDbContext context) : BaseRepository<Users>(context)
    {
        public ReturnData<Users?> Login(string LoginName, string UserPwd)
        {
            ReturnData<Users?> returnData = new ReturnData<Users?>(0);
            List<Users> uList = base.GetList(u => u.LoginName == LoginName && u.UserPwd == UserPwd);
            if (uList.Count > 0)
            {
                returnData.ResultData = uList.FirstOrDefault();
                returnData.IntResult = 1;
            }
            else
            {
                returnData.ResultData = null;
                returnData.IntResult = 0;
            }
            return returnData;
        }

        public ReturnData<Users?> AdminLogin(string LoginName, string UserPwd)
        {
            ReturnData<Users?> returnData = new ReturnData<Users?>(0);
            List<Users> uList = GetList(u => u.LoginName == LoginName && u.UserPwd == UserPwd&& u.Role.RoleName== "管理员");
            if (uList.Count > 0)
            {
                returnData.ResultData = uList.FirstOrDefault();
                returnData.IntResult = 1;
            }
            else
            {
                returnData.ResultData = null;
                returnData.IntResult = 0;
            }
            return returnData;
        }
        public Users GetUserByLoginName(string LoginName)
        {
            Users? users = new Users();
            List<Users> uList = base.GetList(u => u.LoginName== LoginName);
            if (uList.Count > 0)
            {
                return uList.FirstOrDefault();
            }
            else
            {
                return users;
            }
        }
        public new List<Users> GetList()
        {
            return DbSet
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Orders)
                .Include(u => u.Readercomments)
                .Include(u => u.UserState)
                .ToList();
        }
        public new List<Users> GetList(Expression<Func<Users, bool>> predicate)
        {
            return DbSet
                .Where(predicate)   // 生成SQL语句在数据库端过滤
                .AsNoTracking()     //在只读查询中禁用跟踪
                .Include(u => u.Role)
                .Include(u => u.Orders)
                .Include(u => u.Readercomments)
                .Include(u => u.UserState)
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
        public IQueryable<Users> GetPagedList(int pageIndex, int pageSize, out int totalCount,
            Expression<Func<Users, bool>> predicate, 
            Expression<Func<Users, object>> orderBy, bool isAscending = true)
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
            IQueryable<Users> query = DbSet.AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Orders)
                .Include(u => u.Readercomments)
                .Include(u => u.UserState)
                .Where(predicate);

            // 动态排序
            query = isAscending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

            // 分页查询
            IQueryable<Users> items = query
                .Skip(skip)
                .Take(pageSize);

            return items;
        }
        public ReturnData<Users?> Delete(string UserIds)
        {
            ReturnData<Users?> returnData = new ReturnData<Users?>(0);
            if (UserIds.Contains(','))
            {
                string[] strIds = UserIds.Split(',');
                List<int> list = new List<int>();
                foreach (var id in strIds)
                {
                    list.Add(int.Parse(id));
                }
                if (list.Count > 0)
                {
                    returnData.IntResult = DbSet.Where(u => list.Contains(u.UserId)).ExecuteDelete() > 0 ? 1 : 0;
                }
                else
                {
                    returnData.ResultData = null;
                    returnData.IntResult = 0;
                }
                return returnData;
            }
            else if(string.IsNullOrEmpty(UserIds))
            {
                returnData.ResultData = null;
                returnData.IntResult = 0;
                return returnData;
            }
            else
            {
                returnData.IntResult = DbSet.Where(u => u.UserId == int.Parse(UserIds)).ExecuteDelete();
                return returnData;
            }
            
        }
    }
}
