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
    public class PublishersService
    {
        private readonly PublishersRepository modelRepository;

        public PublishersService(PublishersRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        public List<Publishers> GetList()
        {
            List<Publishers> models = modelRepository.GetList();

            return models;
        }
        public List<Publishers> GetList(Expression<Func<Publishers, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public IQueryable<Publishers> GetPagedList(int pageIndex, int pageSize, out int totalCount,Expression<Func<Publishers, bool>> predicate,Expression<Func<Publishers, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount, predicate, orderBy, isAscending = true);
        }
        public ReturnData<Publishers> Add(Publishers model)
        {
            ReturnData<Publishers> returnData = new ReturnData<Publishers>(0);

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
        public ReturnData<Publishers?> Update(Publishers model)
        {
            ReturnData<Publishers?> returnData = new ReturnData<Publishers?>(0);
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
        public ReturnData<Publishers?> Delete(string strids)
        {
            return modelRepository.Delete(strids);
        }
    }
}
