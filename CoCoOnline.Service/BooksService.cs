using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Uitles;
using System.Linq.Expressions;

namespace CoCoOnline.Service
{
    public class BooksService
    {
        private readonly BooksRepository modelRepository;

        public BooksService(BooksRepository modelRepository)
        {
            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        }
        /// <summary>
        /// 获取Books实体
        /// </summary>
        /// <param name="BookId">图书编号</param>
        /// <returns>图书实体</returns>
        public Books? GetModel(int BookId)
        {
            return modelRepository.GetList(u=>u.BookId==BookId).FirstOrDefault();
        }
        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name="model">图书实体</param>
        public ReturnData<Books> Insert(Books model)
        {
            ReturnData<Books> returnData = new ReturnData<Books>(0);
            int result = modelRepository.Insert(model);
            returnData.IntResult = result;
            return returnData;
        }
        /// <summary>
        /// 更新图书
        /// </summary>
        /// <param name="model">图书实体</param>
        /// <returns>更新数量</returns>
        public ReturnData<Books> Update(Books model)
        {
            ReturnData<Books> returnData = new ReturnData<Books>(0);
            int result = modelRepository.Update(model);
            returnData.IntResult = result;
            return returnData;
        }
        /// <summary>
        /// 获取所有图书列表
        /// </summary>
        /// <returns>图书列表</returns>
        public List<Books> GetList()
        {
            List<Books> books = modelRepository.GetList();

            return books;
        }
        /// <summary>
        /// 条件获取图书列表
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns>图书列表</returns>
        public List<Books> GetList(Expression<Func<Books, bool>> predicate)
        {
            return modelRepository.GetList(predicate);
        }
        public IQueryable<Books> GetPagedList(int pageIndex, int pageSize, out int totalCount,Expression<Func<Books, bool>> predicate, Expression<Func<Books, object>> orderBy, bool isAscending = true)
        {
            return modelRepository.GetPagedList(pageIndex, pageSize, out totalCount, predicate, orderBy, isAscending = true);
        }
        public ReturnData<Books?> Delete(string strids)
        {
            return modelRepository.Delete(strids);
        }
    }
}
