using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Http;
using CoCoOnline.Uitles;
using Newtonsoft.Json;
using NuGet.ContentModel;
using Microsoft.AspNetCore.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CoCoOnline.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly BooksRepository _booksRepository;

        public BooksController(BooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }
        #region 普通用户
        /// <summary>
        /// 图书列表首页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public async Task<IActionResult> BookList(int pageIndex = 1)
        {
            string cid = Request.Query["cid"];
            string pid = Request.Query["pid"];

            int pageSize = 15;
            int totalCount = 0;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            BooksService booksService = new BooksService(_booksRepository);
            if (string.IsNullOrEmpty(cid))
            {
                if (string.IsNullOrEmpty(pid))
                {                    
                    var books = booksService.GetPagedList(pageIndex: pageIndex, pageSize: pageSize, out totalCount, predicate: u => u.DeleteFlag == 0, orderBy: u => u.BookId);
                    ViewBag.totalCount = totalCount;
                    ViewBag.totalPage = Math.Ceiling(totalCount * 1.0 / pageSize);
                    ViewBag.cid = "";
                    ViewBag.pid = "";
                    return View(books);                                        
                }
                else
                {
                    int intpid = Convert.ToInt32(pid);
                    var books = booksService.GetPagedList(pageIndex: pageIndex, pageSize: pageSize, out totalCount, predicate: u => u.PublisherId == intpid && u.DeleteFlag == 0, orderBy: u => u.BookId);
                    ViewBag.totalCount = totalCount;
                    ViewBag.totalPage = Math.Ceiling(totalCount * 1.0 / pageSize);
                    ViewBag.cid = "";
                    ViewBag.pid = pid;
                    return View(books);
                }
            }
            else
            {
                int intcid = Convert.ToInt32(cid);
                if (string.IsNullOrEmpty(pid))
                {
                    var books = booksService.GetPagedList(pageIndex: pageIndex, pageSize: pageSize, out totalCount, predicate: u => u.CategoryId == intcid && u.DeleteFlag == 0, orderBy: u => u.BookId);
                    ViewBag.totalCount = totalCount;
                    ViewBag.totalPage = Math.Ceiling(totalCount * 1.0 / pageSize);
                    ViewBag.cid = cid;
                    ViewBag.pid = "";
                    return View(books);
                }
                else
                {
                    int intpid = Convert.ToInt32(pid);
                    var books = booksService.GetPagedList(pageIndex: pageIndex, pageSize: pageSize, out totalCount, predicate: u => u.PublisherId == intpid && u.CategoryId == intcid && u.DeleteFlag == 0, orderBy: u => u.BookId);
                    ViewBag.totalCount = totalCount;
                    ViewBag.totalPage = Math.Ceiling(totalCount * 1.0 / pageSize);
                    ViewBag.cid = cid;
                    ViewBag.pid = pid;
                    return View(books);
                }
            }
        }

        /// <summary>
        /// 图书详情
        /// </summary>
        /// <param name="id">图书编号</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                BooksService booksService = new BooksService(_booksRepository);
                var books = booksService.GetList().OrderByDescending(u => u.PublishDate).FirstOrDefault();
                if (books == null)
                {
                    return NotFound();
                }
                books.Clicks += 1;
                booksService.Update(books);
                ViewData["action"] = "Details";
                return View(books);
            }
            else
            {
                BooksService booksService = new BooksService(_booksRepository);
                var books = booksService.GetModel(id);
                if (books == null)
                {
                    return NotFound();
                }
                books.Clicks += 1;
                booksService.Update(books);
                ViewData["action"] = "Details/"+id;
                return View(books);
            }

        }
        /// <summary>
        /// 购物车试视图
        /// </summary>
        /// <returns></returns>
        public IActionResult ShoppingCart(int pageIndex = 1)
        {
            int pageSize = 15;
            int totalCount = 0;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            //读取Session
            ISession session = HttpContext.Session;
            if (!string.IsNullOrEmpty(session.GetString("ShopCarts")))
            {
                List<ShopCarts>? shopCartsList = JsonConvert.DeserializeObject<List<ShopCarts>>(session.GetString("ShopCarts"));
                totalCount = shopCartsList.Count;
                decimal totalPrice = 0;
                int cartTotal = 0;
                if (totalCount > 0)
                {
                    foreach (var item in shopCartsList)
                    {
                        totalPrice += item.TotalPrice;
                        cartTotal += item.Quantity;
                    }
                }
                ViewBag.totalPrice = totalPrice;
                ViewBag.totalCount = totalCount;
                ViewBag.cartTotal = cartTotal;
                ViewBag.totalPage = Math.Ceiling(totalCount * 1.0 / pageSize);

                // 计算跳过的数据量
                int skip = (pageIndex - 1) * pageSize;

                // 构建基础查询
                IQueryable<ShopCarts> query = shopCartsList.AsQueryable();

                // 分页查询
                List<ShopCarts> items = query
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();
                return View(items);
            }
            else
            {
                return View();
            }
        }
        public IActionResult AddShoppingCart(int BookID)
        {
            ReturnData<ShopCarts> result = new ReturnData<ShopCarts>(0);
            //读取Session
            ISession session = HttpContext.Session;
            //如果购物车数据为空，需要新增
            if (string.IsNullOrEmpty(session.GetString("ShopCarts")))
            {
                List<ShopCarts> shopCartsList = new List<ShopCarts>();
                ShopCarts shopCarts = new ShopCarts();
                //用户信息失效，需要重新登录
                if (string.IsNullOrEmpty(session.GetString("Users")))
                {
                    result.IntResult = 0;
                    return Json(result);
                }
                else
                {
                    //购物车新增
                    Users user = JsonConvert.DeserializeObject<Users>(session.GetString("Users"));
                    shopCarts.UserId = user.UserId;
                    BooksService booksService = new BooksService(_booksRepository);
                    Books? books = booksService.GetModel(BookID);
                    shopCarts.BookId = books.BookId;
                    shopCarts.BookTitle = books.BookTitle;
                    shopCarts.BookAuthor = books.BookAuthor;
                    shopCarts.CategoryId = books.CategoryId;
                    shopCarts.PublisherId = books.PublisherId;
                    shopCarts.UnitPrice = books.UnitPrice;
                    shopCarts.PublishDate = books.PublishDate;
                    shopCarts.Quantity = 1;
                    shopCarts.TotalPrice = books.UnitPrice * 1;
                    shopCartsList.Add(shopCarts);
                    session.SetString("ShopCarts", JsonConvert.SerializeObject(shopCartsList));
                    result.IntResult = 1;
                    result.ResultData = shopCarts;
                    return Json(result);
                }
            }
            else
            {
                List<ShopCarts>? shopCartsList = JsonConvert.DeserializeObject<List<ShopCarts>>(session.GetString("ShopCarts"));
                ShopCarts? shopCarts = shopCartsList.Find(u => u.BookId == BookID);
                //如果购物车中没有这本书，就需要新增
                if (shopCarts is null)
                {
                    BooksService booksService = new BooksService(_booksRepository);
                    Books? books = booksService.GetModel(BookID);
                    shopCarts = new ShopCarts();
                    shopCarts.BookId = books.BookId;
                    shopCarts.BookTitle = books.BookTitle;
                    shopCarts.BookAuthor = books.BookAuthor;
                    shopCarts.CategoryId = books.CategoryId;
                    shopCarts.PublisherId = books.PublisherId;
                    shopCarts.UnitPrice = books.UnitPrice;
                    shopCarts.PublishDate = books.PublishDate;
                    shopCarts.Quantity = 1;
                    shopCarts.TotalPrice = shopCarts.UnitPrice * shopCarts.Quantity;
                }
                else//如果购物车中有这本书，就需要更新数量和总价
                {
                    shopCartsList.Remove(shopCarts);
                    shopCarts.Quantity += 1;
                    shopCarts.TotalPrice = shopCarts.UnitPrice * shopCarts.Quantity;
                }
                shopCartsList.Add(shopCarts);
                session.Remove("ShopCarts");
                session.SetString("ShopCarts", JsonConvert.SerializeObject(shopCartsList));
                result.IntResult = 1;
                result.ResultData = shopCarts;
                return Json(result);
            }
        }
        public IActionResult GetShopCart()
        {
            int BookID = Convert.ToInt32(Request.Query["BookID"]);
            ReturnData<ShopCarts> result = new ReturnData<ShopCarts>(0);
            //读取Session
            ISession session = HttpContext.Session;
            //如果购物车数据为空，返回空
            if (string.IsNullOrEmpty(session.GetString("ShopCarts")))
            {
                ShopCarts shopCarts = new ShopCarts();
                //用户信息失效，需要重新登录
                if (string.IsNullOrEmpty(session.GetString("Users")))
                {
                    result.IntResult = 0;
                    return Json(result);
                }
                else
                {
                    result.IntResult = 2;
                    return Json(result);
                }
            }
            else
            {
                List<ShopCarts>? shopCartsList = JsonConvert.DeserializeObject<List<ShopCarts>>(session.GetString("ShopCarts"));
                ShopCarts? shopCarts = shopCartsList.Find(u => u.BookId == BookID);
                //如果购物车中没有这本书，返回空
                if (shopCarts is null)
                {
                    result.IntResult = 2;
                    return Json(result);
                }
                else//如果购物车中有这本书，返回信息
                {
                    result.IntResult = 1;
                    result.ResultData = shopCarts;
                    return Json(result);
                }
            }
        }
        public IActionResult UpdateCart(int BookID, int Quantity)
        {
            ReturnData<ShopCarts> result = new ReturnData<ShopCarts>(0);
            //读取Session
            ISession session = HttpContext.Session;
            //如果购物车数据为空，返回空
            if (string.IsNullOrEmpty(session.GetString("ShopCarts")))
            {
                ShopCarts shopCarts = new ShopCarts();
                //用户信息失效，需要重新登录
                if (string.IsNullOrEmpty(session.GetString("Users")))
                {
                    result.IntResult = 0;
                    return Json(result);
                }
                else
                {
                    result.IntResult = 2;
                    return Json(result);
                }
            }
            else
            {
                List<ShopCarts>? shopCartsList = JsonConvert.DeserializeObject<List<ShopCarts>>(session.GetString("ShopCarts"));
                ShopCarts? shopCarts = shopCartsList.Find(u => u.BookId == BookID);
                //如果购物车中没有这本书，返回空
                if (shopCarts is null)
                {
                    result.IntResult = 2;
                    return Json(result);
                }
                else//如果购物车中有这本书，返回信息
                {
                    //shopCartsList.Remove(shopCarts);
                    result.IntResult = 1;
                    shopCarts.Quantity = Quantity;
                    shopCarts.TotalPrice = Quantity * shopCarts.UnitPrice;
                    //shopCartsList.Add(shopCarts);
                    session.Remove("ShopCarts");
                    session.SetString("ShopCarts", JsonConvert.SerializeObject(shopCartsList));
                    result.ResultData = shopCarts;
                    return Json(result);
                }
            }
        }
        public IActionResult DeleteCart(int BookID)
        {
            ReturnData<ShopCarts> result = new ReturnData<ShopCarts>(0);
            //读取Session
            ISession session = HttpContext.Session;
            //如果购物车数据为空，返回空
            if (string.IsNullOrEmpty(session.GetString("ShopCarts")))
            {
                ShopCarts shopCarts = new ShopCarts();
                //用户信息失效，需要重新登录
                if (string.IsNullOrEmpty(session.GetString("Users")))
                {
                    result.IntResult = 0;
                    return Json(result);
                }
                else
                {
                    result.IntResult = 2;
                    return Json(result);
                }
            }
            else
            {
                List<ShopCarts>? shopCartsList = JsonConvert.DeserializeObject<List<ShopCarts>>(session.GetString("ShopCarts"));
                ShopCarts? shopCarts = shopCartsList.Find(u => u.BookId == BookID);
                //如果购物车中没有这本书，返回空
                if (shopCarts is null)
                {
                    result.IntResult = 2;
                    return Json(result);
                }
                else//如果购物车中有这本书，删除信息
                {
                    shopCartsList.Remove(shopCarts);
                    session.Remove("ShopCarts");
                    session.SetString("ShopCarts", JsonConvert.SerializeObject(shopCartsList));
                    result.IntResult = 1;
                    result.ResultData = shopCarts;
                    return Json(result);
                }
            }
        }
        #endregion

        #region 管理员使用
        // 管理员后台--图书管理主页
        public IActionResult Index()
        {
            return View();
        }
        // 图书管理主页获取图书列表
        public async Task<IActionResult> GetBooksList(int page, int rows)
        {
            BooksService booksService = new BooksService(_booksRepository);
            int pageIndex = page;
            int pageSize = rows;
            string searname = Request.Query["searname"];
            int totalCount;
            IQueryable<Books> modelList = null;
            if (string.IsNullOrEmpty(searname))
            {
                modelList = booksService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.DeleteFlag == 0, u => u.BookId, true);
            }
            else
            {
                modelList = booksService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.BookTitle.Contains(searname) && u.DeleteFlag == 0, u => u.BookId, true);
            }
            var temp = from o in modelList
                       select new
                       {
                           BookId = o.BookId,
                           BookTitle = o.BookTitle,
                           BookAuthor = o.BookAuthor,
                           PublisherId = o.PublisherId,
                           PublisherName = o.Publisher.PublisherName,
                           PublishDate = o.PublishDate,
                           Isbn = o.Isbn,
                           WordsCount = o.WordsCount,
                           UnitPrice = o.UnitPrice,
                           ContentDescription = o.ContentDescription,
                           AuthorDescription = o.AuthorDescription,
                           EditorComment = o.EditorComment,
                           Toc = o.Toc,
                           CategoryId = o.CategoryId,
                           CategoryName = o.Category.CategoryName,
                           Clicks = o.Clicks,
                           ImgUrl = o.ImgUrl,
                           DeleteFlag = o.DeleteFlag
                       };
            return Json(new { total = totalCount, rows = temp });
        }
        // 图书封面图片上传
        [HttpPost]
        public IActionResult UploadFile(IFormFile bookfile)
        {
            ReturnData<string> returnData = new ReturnData<string>(0);
            // 验证文件非空
            if (bookfile == null || bookfile.Length == 0)
            {
                returnData.IntResult = 0;
                returnData.strMessage = "文件为空";
                return Json(returnData);
            }

            // 验证文件类型（示例允许JPG/PNG）
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(bookfile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                returnData.IntResult = 2;
                returnData.strMessage = "仅支持JPG/PNG格式";
                return Json(returnData);
            }

            // 生成唯一文件名（避免重复）
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var savePath = Path.Combine(Directory.GetCurrentDirectory()+"\\wwwroot\\Images\\BookCovers\\", fileName);

            // 异步保存文件
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                bookfile.CopyTo(stream);
                returnData.IntResult = 1;
                returnData.ResultData = "\\Images\\BookCovers\\"+ fileName;
                return Json(returnData);
            }

        }
        [HttpPost]
        public IActionResult Add([Bind("BookId,BookTitle,BookAuthor,PublisherId,PublishDate,Isbn,WordsCount,UnitPrice,ContentDescription,AuthorDescription,EditorComment,Toc,CategoryId,Clicks,ImgUrl,DeleteFlag")] Books books)
        {
            BooksService booksService = new BooksService(_booksRepository);
            books.DeleteFlag = 0;
            ReturnData<Books> returnData = booksService.Insert(books);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("BookId,BookTitle,BookAuthor,PublisherId,PublishDate,Isbn,WordsCount,UnitPrice,ContentDescription,AuthorDescription,EditorComment,Toc,CategoryId,Clicks,ImgUrl,DeleteFlag")] Books books)
        {
            BooksService modelService = new BooksService(_booksRepository);
            ReturnData<Books> returnData = new ReturnData<Books>(0);
            if (id != books.BookId)
            {
                returnData.IntResult = -1;
                return Json(returnData);
            }
            returnData = modelService.Update(books);
            return Json(returnData);
        }
        [HttpPost]
        public IActionResult Delete(string strId)
        {
            BooksService modelService = new BooksService(_booksRepository);
            ReturnData<Books?> returnData = new ReturnData<Books?>(0);
            returnData = modelService.Delete(strId);
            return Json(returnData);
        }
        public IActionResult GetBookTitle()
        {
            BooksService booksService = new BooksService(_booksRepository);
            List<Books> books = booksService.GetList(u => u.DeleteFlag == 0);
            return Json(books);
        }
        #endregion
    }
}
