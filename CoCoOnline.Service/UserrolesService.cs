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
    public class UserrolesService
    {
        private readonly UserrolesRepository modelRepository;

        public UserrolesService(UserrolesRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        public List<Userroles> GetList()
        {
            List<Userroles> models = modelRepository.GetList();

            return models;
        }
        public List<Userroles> GetList(Expression<Func<Userroles, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public IQueryable<Userroles> GetPagedList(int pageIndex, int pageSize, out int totalCount, Expression<Func<Userroles, bool>> predicate, Expression<Func<Userroles, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount, predicate, orderBy, isAscending = true);
        }
        public ReturnData<Userroles> Add(Userroles model)
        {
            ReturnData<Userroles> returnData = new ReturnData<Userroles>(0);

            if (modelRepository.Insert(model) > 0)
            {
                returnData.IntResult = 1;
            }
            else
            {
                returnData.IntResult = 0;
            }
            return returnData;
        }
        public ReturnData<Userroles?> Update(Userroles model)
        {
            ReturnData<Userroles?> returnData = new ReturnData<Userroles?>(0);
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
        public ReturnData<Userroles?> Delete(string strids)
        {
            return modelRepository.Delete(strids);
        }
    }
}
