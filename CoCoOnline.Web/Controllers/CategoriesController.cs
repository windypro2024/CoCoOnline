using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using CoCoOnline.Uitles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoCoOnline.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoriesRepository _categoriesRepository;
        public CategoriesController(CategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetCategoryName()
        {
            CategoriesService categoriesService = new CategoriesService(_categoriesRepository);
            List<Categories> userroles = categoriesService.GetList();
            return Json(userroles);
        }
        // 图书管理主页获取图书列表
        public IActionResult GetCategoryList(int page, int rows)
        {
            CategoriesService modelService = new CategoriesService(_categoriesRepository);
            int pageIndex = page;
            int pageSize = rows;
            string? searname = Request.Query["searname"];
            int totalCount;
            IQueryable<Categories> modelList = null;
            if (string.IsNullOrEmpty(searname))
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.CategoryId>0, u => u.CategoryId, true);
            }
            else
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.CategoryName.Contains(searname) && u.CategoryId > 0, u => u.CategoryId, true);
            }
            var temp = from o in modelList
                       select new
                       {
                           CategoryId = o.CategoryId,
                           CategoryName = o.CategoryName
                           
                       };
            return Json(new { total = totalCount, rows = temp });
        }
        [HttpPost]
        public IActionResult Add([Bind("CategoryId,CategoryName")] Categories model)
        {
            CategoriesService modelService = new CategoriesService(_categoriesRepository);
            ReturnData<Categories> returnData = modelService.Add(model);           
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("CategoryId,CategoryName")] Categories model)
        {
            CategoriesService modelService = new CategoriesService(_categoriesRepository);
            ReturnData<Categories?> returnData = new ReturnData<Categories?>(0);
            if (id != model.CategoryId)
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
            CategoriesService modelService = new CategoriesService(_categoriesRepository);
            ReturnData<Categories?> returnData = new ReturnData<Categories?>(0);
            returnData = modelService.Delete(strId);
            return Json(returnData);
        }
    }
}
