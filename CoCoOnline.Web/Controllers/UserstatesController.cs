using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using CoCoOnline.Uitles;
using Microsoft.AspNetCore.Mvc;

namespace CoCoOnline.Web.Controllers
{
    public class UserstatesController : Controller
    {
        private readonly UserstatesRepository _userstatesRepository;
        public UserstatesController(UserstatesRepository userstatesRepository)
        {
            _userstatesRepository = userstatesRepository;
        }
        #region 后台管理使用
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetStateName()
        {
            UserstatesService modelService = new UserstatesService(_userstatesRepository);
            List<Userstates> userstates = modelService.GetList();
            return Json(userstates);
        }
        // 用户状态管理主页获取状态列表
        public IActionResult GetStateList(int page, int rows)
        {
            UserstatesService modelService = new UserstatesService(_userstatesRepository);
            int pageIndex = page;
            int pageSize = rows;
            string? searname = Request.Query["searname"];
            int totalCount;
            IQueryable<Userstates> modelList = null;
            if (string.IsNullOrEmpty(searname))
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.UserStateId > 0, u => u.UserStateId, true);
            }
            else
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.StateName.Contains(searname) && u.UserStateId > 0, u => u.UserStateId, true);
            }
            var temp = from o in modelList
                       select new
                       {
                           UserStateId = o.UserStateId,
                           StateName = o.StateName

                       };
            return Json(new { total = totalCount, rows = temp });
        }
        [HttpPost]
        public IActionResult Add([Bind("UserStateId,StateName")] Userstates model)
        {
            UserstatesService modelService = new UserstatesService(_userstatesRepository);
            ReturnData<Userstates> returnData = modelService.Add(model);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("UserStateId,StateName")] Userstates model)
        {
            UserstatesService modelService = new UserstatesService(_userstatesRepository);
            ReturnData<Userstates?> returnData = new ReturnData<Userstates?>(0);
            if (id != model.UserStateId)
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
            UserstatesService modelService = new UserstatesService(_userstatesRepository);
            ReturnData<Userstates?> returnData = new ReturnData<Userstates?>(0);
            returnData = modelService.Delete(strId);
            return Json(returnData);
        }
        #endregion
    }
}
