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
    public class CategoriesService
    {
        private readonly CategoriesRepository modelRepository;

        public CategoriesService(CategoriesRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        public List<Categories> GetList()
        {
            List<Categories> models = modelRepository.GetList();

            return models;
        }
        public List<Categories> GetList(Expression<Func<Categories, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public IQueryable<Categories> GetPagedList(int pageIndex, int pageSize, out int totalCount, Expression<Func<Categories, bool>> predicate, Expression<Func<Categories, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount, predicate, orderBy, isAscending = true);
        }
        public ReturnData<Categories> Add(Categories model)
        {
            ReturnData<Categories> returnData = new ReturnData<Categories>(0);

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
        public ReturnData<Categories?> Update(Categories model)
        {
            ReturnData<Categories?> returnData = new ReturnData<Categories?>(0);
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
        public ReturnData<Categories?> Delete(string strids)
        {
            return modelRepository.Delete(strids);
        }
    }
}
