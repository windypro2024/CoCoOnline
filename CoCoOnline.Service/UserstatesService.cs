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
    public class UserstatesService
    {
        private readonly UserstatesRepository modelRepository;

        public UserstatesService(UserstatesRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        public List<Userstates> GetList()
        {
            List<Userstates> models = modelRepository.GetList();

            return models;
        }
        public List<Userstates> GetList(Expression<Func<Userstates, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public IQueryable<Userstates> GetPagedList(int pageIndex, int pageSize, out int totalCount, Expression<Func<Userstates, bool>> predicate, Expression<Func<Userstates, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount, predicate, orderBy, isAscending = true);
        }
        public ReturnData<Userstates> Add(Userstates model)
        {
            ReturnData<Userstates> returnData = new ReturnData<Userstates>(0);

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
        public ReturnData<Userstates?> Update(Userstates model)
        {
            ReturnData<Userstates?> returnData = new ReturnData<Userstates?>(0);
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
        public ReturnData<Userstates?> Delete(string strids)
        {
            return modelRepository.Delete(strids);
        }
    }
}
