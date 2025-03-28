using CoCoOnline.Models;
using CoCoOnline.Uitles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoCoOnline.Repository
{
    public class OrdersRepository(AppDbContext context) : BaseRepository<Orders>(context)
    {
        /// <summary>
        /// 异步向Order和Orderbooks都要添加数据
        /// </summary>
        /// <param name="orders">Order</param>
        /// <param name="orderbooks">Orderbooks</param>
        /// <returns></returns>
        public async Task<ReturnData<Orders>> Insert(Orders orders, List<Orderbooks> orderbooks)
        {
            ReturnData<Orders> returnData = new ReturnData<Orders>(0);
            // 使用事务
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 添加数据到 Orders
                    await context.Orders.AddAsync(orders);

                    // 保存数据，确保生成OrderID
                    await context.SaveChangesAsync();

                    var orderId = orders.OrderId;
                    for (int i = 0; i < orderbooks.Count; i++)
                    {
                        orderbooks.ElementAt(i).OrderId = orderId;
                    }
                    // 添加数据到 Orderbooks
                    await context.Orderbooks.AddRangeAsync(orderbooks);

                    // 保存更改（此时数据尚未提交到数据库）
                    await context.SaveChangesAsync();

                    // 提交事务
                    await transaction.CommitAsync();

                    returnData.IntResult = 1;
                    return returnData;
                }
                catch (Exception ex)
                {
                    // 回滚事务
                    await transaction.RollbackAsync();
                    returnData.IntResult = -1;
                    returnData.strMessage = ex.Message;
                    return returnData;
                }
            }
        }
        /// <summary>
        /// 异步删除Orderbooks和Order数据
        /// </summary>
        /// <param name="orders">Order</param>
        /// <param name="orderbooks">Orderbooks</param>
        /// <returns></returns>
        public async Task<ReturnData<Orders>> Delete(int ordersId)
        {
            ReturnData<Orders> returnData = new ReturnData<Orders>(0);
            // 使用事务
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 直接批量删除 Orderbooks 数据（无需加载实体）
                    await context.Orderbooks
                        .Where(x => x.OrderId == ordersId)
                        .ExecuteDeleteAsync();

                    // 2. 直接批量删除 Orders 数据
                    await context.Orders
                        .Where(x => x.OrderId == ordersId)
                        .ExecuteDeleteAsync();

                    // 3. 提交事务
                    await transaction.CommitAsync();

                    returnData.IntResult = 1;
                    return returnData;
                }
                catch (Exception ex)
                {
                    // 回滚事务
                    await transaction.RollbackAsync();
                    returnData.IntResult = -1;
                    returnData.strMessage = ex.Message;
                    return returnData;
                }
            }
        }
        /// <summary>
        /// 批量异步删除Orderbooks和Order数据
        /// </summary>
        /// <param name="orders">Order</param>
        /// <param name="orderbooks">Orderbooks</param>
        /// <returns></returns>
        public async Task<ReturnData<Orders>> Delete(string strIds)
        {
            ReturnData<Orders> returnData = new ReturnData<Orders>(0);
            // 使用事务
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
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
                            int restult = 0;
                            // 1. 直接批量删除 Orderbooks 数据（无需加载实体）
                            restult = await context.Orderbooks
                                .Where(u => list.Contains(u.OrderId))
                                .ExecuteDeleteAsync();

                            // 2. 直接批量删除 Orders 数据
                            restult += await context.Orders
                                .Where(u => list.Contains(u.OrderId))
                                .ExecuteDeleteAsync();

                            // 3. 提交事务
                            await transaction.CommitAsync();
                            returnData.IntResult = restult > 0 ? 1 : 0;
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
                        // 1. 直接批量删除 Orderbooks 数据（无需加载实体）
                        await context.Orderbooks
                            .Where(x => x.OrderId == int.Parse(strIds))
                            .ExecuteDeleteAsync();

                        // 2. 直接批量删除 Orders 数据
                        await context.Orders
                            .Where(x => x.OrderId == int.Parse(strIds))
                            .ExecuteDeleteAsync();

                        // 3. 提交事务
                        await transaction.CommitAsync();

                        returnData.IntResult = 1;
                        return returnData;
                    }
                }
                catch (Exception ex)
                {
                    // 回滚事务
                    await transaction.RollbackAsync();
                    returnData.IntResult = -1;
                    returnData.strMessage = ex.Message;
                    return returnData;
                }
            }
        }
        public new List<Orders> GetList()
        {
            return DbSet
                .AsNoTracking()
                .Include(u => u.Orderbooks)
                .Include(u => u.User)
                .ToList();
        }
        public new List<Orders> GetList(Expression<Func<Orders, bool>> predicate)
        {
            return DbSet
                .Where(predicate)   // 生成SQL语句在数据库端过滤
                .AsNoTracking()     //在只读查询中禁用跟踪
                .Include(u => u.Orderbooks)
                .Include(u => u.User)
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
        public IQueryable<Orders> GetPagedList(int pageIndex, int pageSize, out int totalCount, Expression<Func<Orders, bool>> predicate, Expression<Func<Orders, object>> orderBy, bool isAscending = true)
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
            IQueryable<Orders> query = DbSet.AsNoTracking()
                .Include(u => u.Orderbooks)
                .Include(u => u.User)
                .Where(predicate);

            // 动态排序
            query = isAscending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

            // 分页查询
            IQueryable<Orders> items = query
                .Skip(skip)
                .Take(pageSize);

            return items;
        }
    }
}
