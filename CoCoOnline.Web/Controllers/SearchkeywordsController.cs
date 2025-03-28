using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using CoCoOnline.Uitles;
using Microsoft.AspNetCore.Mvc;

namespace CoCoOnline.Web.Controllers
{
    public class SearchkeywordsController : Controller
    {
        private readonly SearchkeywordsRepository _searchkeywordsRepository;
        public SearchkeywordsController(SearchkeywordsRepository searchkeywordsRepository)
        {
            _searchkeywordsRepository = searchkeywordsRepository;
        }
        #region 后台管理使用
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetSearchkeywords()
        {
            SearchkeywordsService modelService = new SearchkeywordsService(_searchkeywordsRepository);
            List<Searchkeywords> searchkeywords = modelService.GetList();
            return Json(searchkeywords);
        }
        // 搜索管理主页获取搜索关键词列表
        public IActionResult GetSearchkeywordsList(int page, int rows)
        {
            SearchkeywordsService modelService = new SearchkeywordsService(_searchkeywordsRepository);
            int pageIndex = page;
            int pageSize = rows;
            string? searname = Request.Query["searname"];
            int totalCount;
            IQueryable<Searchkeywords> modelList = null;
            if (string.IsNullOrEmpty(searname))
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.Id > 0, u => u.Id, true);
            }
            else
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.Keyword.Contains(searname) && u.Id > 0, u => u.Id, true);
            }
            var temp = from o in modelList
                       select new
                       {
                           Id = o.Id,
                           Keyword = o.Keyword,
                           SearchCount = o.SearchCount

                       };
            return Json(new { total = totalCount, rows = temp });
        }
        [HttpPost]
        public IActionResult Add([Bind("Id,Keyword,SearchCount")] Searchkeywords model)
        {
            SearchkeywordsService modelService = new SearchkeywordsService(_searchkeywordsRepository);
            ReturnData<Searchkeywords> returnData = modelService.Add(model);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,Keyword,SearchCount")] Searchkeywords model)
        {
            SearchkeywordsService modelService = new SearchkeywordsService(_searchkeywordsRepository);
            ReturnData<Searchkeywords?> returnData = new ReturnData<Searchkeywords?>(0);
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
            SearchkeywordsService modelService = new SearchkeywordsService(_searchkeywordsRepository);
            ReturnData<Searchkeywords?> returnData = new ReturnData<Searchkeywords?>(0);
            returnData = modelService.Delete(strId);
            return Json(returnData);
        }
        #endregion
    }
}
