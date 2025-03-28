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
    public class SearchkeywordsService
    {
        private readonly SearchkeywordsRepository modelRepository;

        public SearchkeywordsService(SearchkeywordsRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        public List<Searchkeywords> GetList()
        {
            List<Searchkeywords> models = modelRepository.GetList();

            return models;
        }
        public List<Searchkeywords> GetList(Expression<Func<Searchkeywords, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public IQueryable<Searchkeywords> GetPagedList(int pageIndex, int pageSize, out int totalCount, Expression<Func<Searchkeywords, bool>> predicate, Expression<Func<Searchkeywords, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount, predicate, orderBy, isAscending = true);
        }
        public ReturnData<Searchkeywords> Add(Searchkeywords model)
        {
            ReturnData<Searchkeywords> returnData = new ReturnData<Searchkeywords>(0);

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
        public ReturnData<Searchkeywords?> Update(Searchkeywords model)
        {
            ReturnData<Searchkeywords?> returnData = new ReturnData<Searchkeywords?>(0);
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
        public ReturnData<Searchkeywords?> Delete(string strids)
        {
            return modelRepository.Delete(strids);
        }
    }
}
