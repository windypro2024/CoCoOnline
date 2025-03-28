using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoCoOnline.Web.Models;
using CoCoOnline.Service;
using CoCoOnline.Repository;
using Newtonsoft.Json;
using CoCoOnline.Models;
using Microsoft.AspNetCore.Html;
using System.Text;
using Newtonsoft.Json.Linq;
using System;
using CoCoOnline.Uitles;

namespace CoCoOnline.Web.Controllers;

public class HomeController : Controller
{
    private readonly BooksRepository _booksRepository;
    private readonly CategoriesRepository _categoriesRepository;
    private readonly PublishersRepository _publishersRepository;
    private readonly UsersRepository _usersRepository;

    public HomeController(BooksRepository booksRepository, CategoriesRepository categoriesRepository, PublishersRepository publishersRepository, UsersRepository usersRepository    )
    {
        _booksRepository = booksRepository;
        _categoriesRepository = categoriesRepository;
        _publishersRepository = publishersRepository;
        _usersRepository = usersRepository;
    }
    public IActionResult Index()
    {
        //判断用户是否登录，如果已经登录，用户区域显示用户信息，否则显示登录
        var userinfo = LoginShowHtml();
        ViewBag.DynamicHtml = userinfo;
        BooksService booksService = new BooksService(_booksRepository);
        CategoriesService categoriesService = new CategoriesService(_categoriesRepository);
        PublishersService publishersService = new PublishersService(_publishersRepository);
        var model = new HomeViewModel
        {
            LastBook = booksService.GetList().OrderByDescending(u => u.PublishDate).FirstOrDefault(),
            NewBooks = booksService.GetList().OrderByDescending(u => u.PublishDate).Take(8),
            OldBooks = booksService.GetList().OrderBy(u => u.PublishDate).Take(6),
            ClickBooks = booksService.GetList().OrderByDescending(u => u.Clicks).Take(10),
            Publishers = publishersService.GetList().Take(10),
            Categories = categoriesService.GetList().Take(20),
        };
        return View(model);
    }

    public IHtmlContent LoginShowHtml()
    {
        ISession session = HttpContext.Session;
        HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
        if (string.IsNullOrEmpty(session.GetString("Users")))
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
                    session = HttpContext.Session;
                    session.SetString("Users", JsonConvert.SerializeObject(users));
                }
            }
            else
            {
                htmlContentBuilder.AppendHtml("<form id=\"myForm\" method=\"post\" novalidate>");
                htmlContentBuilder.AppendHtml("<div style=\"height: 340px;\">");
                htmlContentBuilder.AppendHtml("Email地址或昵称:<br />");
                htmlContentBuilder.AppendHtml("<input type=\"text\" name=\"LoginName\" class=\"easyui-textbox\" required=\"true\" />");
                htmlContentBuilder.AppendHtml("<br />密码:<br />");
                htmlContentBuilder.AppendHtml("<input type=\"password\" name=\"UserPwd\" class=\"easyui-textbox\" required=\"true\" />");
                htmlContentBuilder.AppendHtml("<br />");
                htmlContentBuilder.AppendHtml("<input type=\"submit\" class=\"easyui-linkbutton\" value=\"登录\" style=\"width:80px;height:30px;\" /><br />");
                htmlContentBuilder.AppendHtml("你不是可可网用户？<br />");
                htmlContentBuilder.AppendHtml("<a href=\"/Users/Register\">创建一个新用户>></a>");
                htmlContentBuilder.AppendHtml("</div>");
                htmlContentBuilder.AppendHtml("</form>");
            }
                
        }
        else
        {
            Users user = JsonConvert.DeserializeObject<Users>(session.GetString("Users"));
            htmlContentBuilder.AppendHtml("<div style='height: 340px;'>");
            htmlContentBuilder.AppendHtml("<div class='userItem mt30'>欢迎：<span style='color:dodgerblue'>" + user.UserName + "</span></div>");
            var loginTime = session.GetString("UsersLoginTime");
            htmlContentBuilder.AppendHtml("<div class='userItem mt5'>登录日期：<br /><span>" + loginTime + "</span></div>");
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            string ipstr = ip ?? "::1";
            htmlContentBuilder.AppendHtml("<div class='userItem mt20'>登录IP：<br /><span>" + ipstr + "</span></div>");
            htmlContentBuilder.AppendHtml("<div class='userItem mt40'>");
            htmlContentBuilder.AppendHtml("<a class=\" easyui-linkbutton\" href=\"/Users/LoginOut\">注销</a>");
            htmlContentBuilder.AppendHtml("<a href=\"#\" class=\" easyui-linkbutton\" onclick=\"showInfo()\">修改密码</a>");
            htmlContentBuilder.AppendHtml("</div>");
            htmlContentBuilder.AppendHtml("</div>");
        }
        return htmlContentBuilder;
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public ActionResult CheckUser(string LoginName, string UserPwd)
    {
        UsersService usersService = new UsersService(_usersRepository);
        ReturnData<Users?> returnData = usersService.Login(LoginName, UserPwd);
        if (returnData.IntResult == 1)
        {
            ISession session = HttpContext.Session;
            session.SetString("Users", returnData.ResultData == null ? "" : JsonConvert.SerializeObject(returnData.ResultData));
            session.SetString("UsersLoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            var userinfo = LoginShowHtml();
            ViewBag.DynamicHtml = userinfo;
        }
        return Json(returnData);
    }
    public IActionResult AdminIndex()
    {
        ISession session = HttpContext.Session;
        if (string.IsNullOrEmpty(session.GetString("Admin")))
        {
            ViewBag.Admin = null;
            return Redirect("/Users/AdminLogin");
        }
        else
        {
            Users admin = JsonConvert.DeserializeObject<Users>(session.GetString("Admin"));
            ViewBag.Admin = admin;
            return View();
        }
    }
}
