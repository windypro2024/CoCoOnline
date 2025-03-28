using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using CoCoOnline.Uitles;
using Microsoft.AspNetCore.Mvc;

namespace CoCoOnline.Web.Controllers
{
    public class PublishersController : Controller
    {
        private readonly PublishersRepository _publishersRepository;
        public PublishersController(PublishersRepository publishersRepository)
        {
            _publishersRepository = publishersRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetPublisherName()
        {
            PublishersService publishersService = new PublishersService(_publishersRepository);
            List<Publishers> publishers = publishersService.GetList();
            return Json(publishers);
        }
        // 出版社管理主页获取出版社列表
        public IActionResult GetPublisherList(int page, int rows)
        {
            PublishersService modelService = new PublishersService(_publishersRepository);
            int pageIndex = page;
            int pageSize = rows;
            string? searname = Request.Query["searname"];
            int totalCount;
            IQueryable<Publishers> modelList = null;
            if (string.IsNullOrEmpty(searname))
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.PublisherId > 0, u => u.PublisherId, true);
            }
            else
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.PublisherName.Contains(searname) && u.PublisherId > 0, u => u.PublisherId, true);
            }
            var temp = from o in modelList
                       select new
                       {
                           PublisherId = o.PublisherId,
                           PublisherName = o.PublisherName

                       };
            return Json(new { total = totalCount, rows = temp });
        }
        [HttpPost]
        public IActionResult Add([Bind("PublisherId,PublisherName")] Publishers model)
        {
            PublishersService modelService = new PublishersService(_publishersRepository);
            ReturnData<Publishers> returnData = modelService.Add(model);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("PublisherId,PublisherName")] Publishers model)
        {
            PublishersService modelService = new PublishersService(_publishersRepository);
            ReturnData<Publishers?> returnData = new ReturnData<Publishers?>(0);
            if (id != model.PublisherId)
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
            PublishersService modelService = new PublishersService(_publishersRepository);
            ReturnData<Publishers?> returnData = new ReturnData<Publishers?>(0);
            returnData = modelService.Delete(strId);
            return Json(returnData);
        }
    }
}
