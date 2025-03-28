using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Uitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoCoOnline.Service
{
    public class OrdersService
    {
        private readonly OrdersRepository modelRepository;

        public OrdersService(OrdersRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        public List<Orders> GetList()
        {
            List<Orders> models = modelRepository.GetList();

            return models;
        }
        public List<Orders> GetList(Expression<Func<Orders, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public async Task<ReturnData<Orders>> Insert(Orders orders, List<Orderbooks> orderbooksList)
        {
            return await modelRepository.Insert(orders, orderbooksList);
        }
        public IQueryable<Orders> GetPagedList(int pageIndex, int pageSize,out int totalCount, Expression<Func<Orders, bool>> predicate, Expression<Func<Orders, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount, predicate, orderBy, isAscending);
        }
        public async Task<ReturnData<Orders>> Delete(int ordersId)
        {
            return await modelRepository.Delete(ordersId);
        }
        public ReturnData<Orders?> Update(Orders model)
        {
            ReturnData<Orders?> returnData = new ReturnData<Orders?>(0);
            if (modelRepository.Update(model) > 0)
            {
                returnData.IntResult = 1;
            }
            else
            {
                returnData.IntResult = 0;
            }
            return returnData;
        }
        public async Task<ReturnData<Orders>> Delete(string strids)
        {
            return await modelRepository.Delete(strids);
        }
    }
}
