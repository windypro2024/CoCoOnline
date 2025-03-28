using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using CoCoOnline.Uitles;
using Microsoft.AspNetCore.Mvc;

namespace CoCoOnline.Web.Controllers
{
    public class ReadercommentsController : Controller
    {
        private readonly ReadercommentsRepository _readercommentsRepository;
        public ReadercommentsController(ReadercommentsRepository readercommentsRepository)
        {
            _readercommentsRepository = readercommentsRepository;
        }
        #region 后台管理使用
        public IActionResult Index()
        {
            return View();
        }
        // 读者推荐管理主页获取推荐列表
        public IActionResult GetReadercommentList(int page, int rows)
        {
            ReadercommentsService modelService = new ReadercommentsService(_readercommentsRepository);
            int pageIndex = page;
            int pageSize = rows;
            string? searname = Request.Query["searname"];
            int totalCount;
            IQueryable<Readercomments> modelList = null;
            if (string.IsNullOrEmpty(searname))
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.Id > 0, u => u.Id, true);
            }
            else
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => (u.CommentTitle.Contains(searname)|| u.User.UserName.Contains(searname)|| u.Book.BookTitle.Contains(searname)) , u => u.Id, true);
            }
            var temp = from o in modelList
                       select new
                       {
                           Id = o.Id,
                           BookId = o.BookId,
                           BookTitle = o.Book.BookTitle,
                           BookAuthor = o.Book.BookAuthor,
                           UserId = o.UserId,
                           UserName = o.User.UserName,
                           LoginName = o.User.LoginName,
                           CommentTitle = o.CommentTitle,
                           Comment = o.Comment,
                           Date = o.Date

                       };
            return Json(new { total = totalCount, rows = temp });
        }
        [HttpPost]
        public IActionResult Add([Bind("Id,BookId,UserId,CommentTitle,Comment,Date")] Readercomments model)
        {
            ReadercommentsService modelService = new ReadercommentsService(_readercommentsRepository);
            ReturnData<Readercomments> returnData = modelService.Add(model);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,BookId,UserId,CommentTitle,Comment,Date")] Readercomments model)
        {
            ReadercommentsService modelService = new ReadercommentsService(_readercommentsRepository);
            ReturnData<Readercomments?> returnData = new ReturnData<Readercomments?>(0);
            if (id != model.Id)
            {
                returnData.IntResult = -1;
                return Json(returnData);
            }
            returnData = modelService.Update(model);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Delete(string strId)
        {
            ReadercommentsService modelService = new ReadercommentsService(_readercommentsRepository);
            ReturnData<Readercomments?> returnData = new ReturnData<Readercomments?>(0);
            returnData = modelService.Delete(strId);
            return Json(returnData);
        }
        #endregion
    }
}
