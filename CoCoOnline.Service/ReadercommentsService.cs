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
    public class ReadercommentsService
    {
        private readonly ReadercommentsRepository modelRepository;

        public ReadercommentsService(ReadercommentsRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        public List<Readercomments> GetList()
        {
            List<Readercomments> models = modelRepository.GetList();

            return models;
        }
        public List<Readercomments> GetList(Expression<Func<Readercomments, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public IQueryable<Readercomments> GetPagedList(int pageIndex, int pageSize, out int totalCount, Expression<Func<Readercomments, bool>> predicate, Expression<Func<Readercomments, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount, predicate, orderBy, isAscending = true);
        }
        public ReturnData<Readercomments> Add(Readercomments model)
        {
            ReturnData<Readercomments> returnData = new ReturnData<Readercomments>(0);

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
        public ReturnData<Readercomments?> Update(Readercomments model)
        {
            ReturnData<Readercomments?> returnData = new ReturnData<Readercomments?>(0);
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
        public ReturnData<Readercomments?> Delete(string strids)
        {
            return modelRepository.Delete(strids);
        }
    }
}
