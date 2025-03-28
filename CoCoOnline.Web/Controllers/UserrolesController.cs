using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using CoCoOnline.Uitles;
using Microsoft.AspNetCore.Mvc;

namespace CoCoOnline.Web.Controllers
{
    public class UserrolesController : Controller
    {
        private readonly UserrolesRepository _userrolesRepository;
        public UserrolesController(UserrolesRepository userrolesRepository)
        {
            _userrolesRepository = userrolesRepository;
        }
        #region 后台管理使用
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetRoleName()
        {
            UserrolesService userrolesService = new UserrolesService(_userrolesRepository);
            List<Userroles> userroles = userrolesService.GetList();
            return Json(userroles);
        }
        // 角色管理主页获取角色列表
        public IActionResult GetRoleList(int page, int rows)
        {
            UserrolesService modelService = new UserrolesService(_userrolesRepository);
            int pageIndex = page;
            int pageSize = rows;
            string? searname = Request.Query["searname"];
            int totalCount;
            IQueryable<Userroles> modelList = null;
            if (string.IsNullOrEmpty(searname))
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.RoleId > 0, u => u.RoleId, true);
            }
            else
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.RoleName.Contains(searname) && u.RoleId > 0, u => u.RoleId, true);
            }
            var temp = from o in modelList
                       select new
                       {
                           RoleId = o.RoleId,
                           RoleName = o.RoleName

                       };
            return Json(new { total = totalCount, rows = temp });
        }
        [HttpPost]
        public IActionResult Add([Bind("RoleId,RoleName")] Userroles model)
        {
            UserrolesService modelService = new UserrolesService(_userrolesRepository);
            ReturnData<Userroles> returnData = modelService.Add(model);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("RoleId,RoleName")] Userroles model)
        {
            UserrolesService modelService = new UserrolesService(_userrolesRepository);
            ReturnData<Userroles?> returnData = new ReturnData<Userroles?>(0);
            if (id != model.RoleId)
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
            UserrolesService modelService = new UserrolesService(_userrolesRepository);
            ReturnData<Userroles?> returnData = new ReturnData<Userroles?>(0);
            returnData = modelService.Delete(strId);
            return Json(returnData);
        }
        #endregion
    }
}
