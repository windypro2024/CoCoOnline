using CoCoOnline.Models;
using CoCoOnline.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoCoOnline.Service
{
    public class OrderbooksService
    {
        private readonly OrderbooksRepository modelRepository;

        public OrderbooksService(OrderbooksRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        public List<Orderbooks> GetList()
        {
            List<Orderbooks> models = modelRepository.GetList();

            return models;
        }
        public List<Orderbooks> GetList(Expression<Func<Orderbooks, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public IQueryable<Orderbooks> GetPagedList(int pageIndex, int pageSize, out int totalCount,Expression<Func<Orderbooks, bool>> predicate, Expression<Func<Orderbooks, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount,predicate, orderBy);
        }
        public int Insert(Orderbooks model)
        {
            return modelRepository.Insert(model);
        }
    }
}
