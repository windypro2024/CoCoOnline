using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using CoCoOnline.Uitles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace CoCoOnline.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UsersRepository _usersRepository;
        private readonly UserrolesRepository _userrolesRepository;
        private readonly UserstatesRepository _userstatesRepository;
        public UsersController(UsersRepository usersRepository, UserrolesRepository userrolesRepository, UserstatesRepository userstatesRepository)
        {
            _usersRepository = usersRepository;
            _userrolesRepository = userrolesRepository;
            _userstatesRepository = userstatesRepository;
        }
        #region 普通用户使用
        public IActionResult Login()
        {
            //先检查cookie，如果cookie存在直接登录成功
            CheckCookieInfo();
            return View(new Users());
        }
        /// <summary>
        /// 校验用户名和密码。参数名称要与form中name名称一致
        /// </summary>
        /// <param name="LoginName">用户名</param>
        /// <param name="UserPwd">密码</param>
        /// <param name="ckTwoWeek">2周内不用登录</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CheckUser(string LoginName, string UserPwd,string ckTwoWeek)
        {
            UsersService usersService = new UsersService(_usersRepository);
            ReturnData<Users?> returnData = usersService.Login(LoginName, UserPwd);
            if (returnData.IntResult == 1)
            {
                ISession session = HttpContext.Session;
                session.SetString("Users", returnData.ResultData == null ? "" : JsonConvert.SerializeObject(returnData.ResultData));
                session.SetString("UsersLoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //判断用户是否选择了2周内不用登录,写入客户端cookie
                if (!string.IsNullOrEmpty(ckTwoWeek))
                {
                    // 设置Cookie参数
                    CookieOptions option = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(14),  // Cookie 过期时间14天
                        HttpOnly = true,                     // 防止客户端 JavaScript 访问此 Cookie
                        Secure = true                        // 仅在 HTTPS 下传输此 Cookie
                    };
                    Response.Cookies.Append("cookie1", LoginName, option);
                    Response.Cookies.Append("cookie2", SecurityTools.Md5String(UserPwd), option);
                }
            }

            return Json(returnData);
        }
        /// <summary>
        /// 检查cookie
        /// </summary>
        public void CheckCookieInfo()
        {
            //获取登录用户名cookie1，MD5加密之后的密码cookie2
            if (Request.Cookies["cookie1"] != null && Request.Cookies["cookie2"] != null)
            {
                string cookieUserName = Request.Cookies["cookie1"];
                string cookieUserPwd = Request.Cookies["cookie2"];
                //获取Users实体
                UsersService usersService = new UsersService(_usersRepository);
                Users users = usersService.GetUserByLoginName(cookieUserName);
                //得到用户密码
                var stuPassWord = users.UserPwd;

                //注意：要将用户密码加密以后Cookies比较
                //调用工具类SecurityTools中的静态方法Md5String，加密字符串
                string md5Pwd = SecurityTools.Md5String(stuPassWord);
                if (md5Pwd == cookieUserPwd)
                {
                    //记录Session
                    ISession session = HttpContext.Session;
                    session.SetString("Users", JsonConvert.SerializeObject(users));
                    //跳转到首页
                    Response.Redirect("/Home/Index");
                }
            }
        }
        public void LoginOut()
        {
            //删除Sessiong和cookie
            ISession session = HttpContext.Session;
            session.Remove("Users");

            Response.Cookies.Delete("cookie1");
            Response.Cookies.Delete("cookie2");

            Response.Redirect("/Users/Login");
        }
        [HttpPost]
        public IActionResult ChangePwd(string UserName, string UserPwd,string NewUserPwd,string ReUserPwd)
        {
            ISession session = HttpContext.Session;
            ReturnData<Users?> returnData2 = new ReturnData<Users?>(0);
            if (!string.IsNullOrEmpty(session.GetString("Users")))
            {
                Users user = JsonConvert.DeserializeObject<Users>(session.GetString("Users"));
                string LoginName = user.LoginName;
                UsersService usersService = new UsersService(_usersRepository);
                ReturnData<Users?> returnData = usersService.Login(LoginName, UserPwd);
                
                if (returnData.IntResult == 1)
                {
                    
                    if (NewUserPwd == ReUserPwd)
                    {
                        user.UserPwd = NewUserPwd;
                        returnData2 = usersService.Update(user);
                        //清空session
                        session.Clear();
                        //删除cookie
                        Response.Cookies.Delete("cookie1");
                        Response.Cookies.Delete("cookie2");
                        return Json(returnData2);
                    }
                    else
                    {
                        returnData2.IntResult = 2;//两次密码不一致
                        return Json(returnData2);
                    }
                }
                else
                {
                    returnData2.IntResult = 0;//输入密码错误
                    return Json(returnData2);
                }
            }
            else
            {
                returnData2.IntResult = -1;//重新登录
                return Json(returnData2);
            }
        }
        
        public IActionResult Register()
        {
            ViewData["RoleId"] = new SelectList(_userrolesRepository.GetList(), "RoleId", "RoleName");
            ViewData["UserStateId"] = new SelectList(_userstatesRepository.GetList(), "UserStateId", "StateName");
            return View();
        }
        [HttpPost]
        public IActionResult Register([Bind("UserId,LoginName,UserName,UserPwd,Address,Phone,Email,RoleId,UserStateId")] Users users)
        {
            UsersService usersService = new UsersService(_usersRepository);
            users.RegDate = DateTime.Now;
            ReturnData<Users?> returnData = usersService.Register(users);
            ViewData["RoleId"] = new SelectList(_userrolesRepository.GetList(), "RoleId", "RoleName");
            ViewData["UserStateId"] = new SelectList(_userstatesRepository.GetList(), "UserStateId", "StateName");
            return Json(returnData);
        }
        #endregion

        #region 管理员使用
        // 后台用户管理主视图
        public IActionResult Index()
        {
            //UsersService usersService = new UsersService(_usersRepository);
            //return View(usersService.GetList());
            return View();
        }

        // 管理员登录视图
        public IActionResult AdminLogin()
        {
            return View();
        }
        // 管理员身份校验
        [HttpPost]
        public IActionResult CheckAdmin(string LoginName, string UserPwd)
        {
            UsersService usersService = new UsersService(_usersRepository);
            ReturnData<Users?> returnData = usersService.AdminLogin(LoginName, UserPwd);
            if (returnData.IntResult == 1)
            {
                ISession session = HttpContext.Session;
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                session.SetString("Admin", returnData.ResultData == null ? "" : JsonConvert.SerializeObject(returnData.ResultData, settings));
                session.SetString("AdminLoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                return Json(returnData);
            }
            else
            {
                returnData.IntResult = 0;
                return Json(returnData);
            }                
        }
        public IActionResult GetUserList(int page, int rows)
        {
            UsersService usersService = new UsersService(_usersRepository);
            int pageIndex = page;// int.Parse(Request.Query["page"]);
            int pageSize = rows;// int.Parse(Request.Query["rows"]);
            string userName = Request.Query["username"];
            int totalCount;
            IQueryable<Users> userList = null;
            if (string.IsNullOrEmpty(userName))
            {
                userList = usersService.GetPagedList(pageIndex,pageSize, out totalCount, u =>u.UserId>0, u => u.UserId, true);
            }
            else
            {
                userList = usersService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.UserName.Contains(userName), u => u.UserId, true);
            }
            var temp = from o in userList
                       select new
                       {
                           UserId = o.UserId,
                           LoginName = o.LoginName,
                           UserName = o.UserName,
                           UserPwd = o.UserPwd,
                           Address = o.Address,
                           Phone = o.Phone,
                           Email = o.Email,
                           RoleId = o.RoleId,
                           RoleName = o.Role.RoleName,
                           RegDate = o.RegDate,
                           UserStateId = o.UserState.UserStateId,
                           StateName = o.UserState.StateName
                       };
            return Json(new { total = totalCount, rows = temp });
        }
        [HttpPost]
        public IActionResult Add([Bind("UserId,LoginName,UserName,UserPwd,Address,Phone,Email,RoleId,UserStateId")] Users users)
        {
            UsersService usersService = new UsersService(_usersRepository);
            users.RegDate = DateTime.Now;
            ReturnData<Users?> returnData = usersService.Register(users);
            ViewData["RoleId"] = new SelectList(_userrolesRepository.GetList(), "RoleId", "RoleName");
            ViewData["UserStateId"] = new SelectList(_userstatesRepository.GetList(), "UserStateId", "StateName");
            return Json(returnData);
        }
        // 管理员退出
        public void AdminLoginOut()
        {
            //删除Sessiong和cookie
            ISession session = HttpContext.Session;
            session.Remove("Admin");

            Response.Redirect("/Users/AdminLogin");
        }        

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("UserId,LoginName,UserName,UserPwd,Address,Phone,Email,RoleId,UserStateId,RegDate")] Users users)
        {
            UsersService usersService = new UsersService(_usersRepository);
            ReturnData<Users> returnData = new ReturnData<Users>(0);
            if (id != users.UserId)
            {
                returnData.IntResult = -1;
                return Json(returnData);
            }
            returnData = usersService.Update(users);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Delete(string strId)
        {
            UsersService usersService = new UsersService(_usersRepository);
            ReturnData<Users> returnData = new ReturnData<Users>(0);
            returnData = usersService.Delete(strId);
            return Json(returnData);
        }
        public IActionResult GetUsersName()
        {
            UsersService usersService = new UsersService(_usersRepository);
            List<Users> users = usersService.GetList();
            return Json(users);
        }
        #endregion
    }
}
