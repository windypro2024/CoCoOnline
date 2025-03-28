using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using CoCoOnline.Uitles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static NuGet.Packaging.PackagingConstants;

namespace CoCoOnline.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrdersRepository _orderRepository;
        private readonly OrderbooksRepository _orderbooksRepository;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public OrdersController(OrdersRepository orderRepository, OrderbooksRepository orderbooksRepository, HttpClient httpClient, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _orderbooksRepository = orderbooksRepository;
            _httpClient = httpClient;
            _apiUrl = configuration["ConnectionStrings:DefaultApiURL"];
        }
        #region 普通用户使用
        /// <summary>
        /// 结算视图
        /// </summary>
        /// <returns></returns>
        public IActionResult Balance(int pageIndex = 1)
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
                Users user = JsonConvert.DeserializeObject<Users>(session.GetString("Users"));
                ViewBag.Users = user;
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
            return View();
        }
        public IActionResult Detail(int? id, int pageIndex = 1)
        {
            int? orderid = id;
            int pageSize = 15;
            int totalCount = 0;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;            
            //读取Session
            ISession session = HttpContext.Session;
            if (!string.IsNullOrEmpty(session.GetString("Users")))
            {
                OrdersService ordersService = new OrdersService(_orderRepository);
                OrderbooksService orderbooksService = new OrderbooksService(_orderbooksRepository);
                Users user = JsonConvert.DeserializeObject<Users>(session.GetString("Users"));
                Orders orders = ordersService.GetList(u => u.OrderId == orderid && u.UserId == user.UserId).FirstOrDefault();
                ViewBag.Users = user;
                ViewBag.Orders = orders;
                IQueryable<Orderbooks> orderbooksList = orderbooksService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.OrderId == orderid, u => u.Order.OrderDate);
                int cartTotal = 0;
                if (totalCount > 0)
                {
                    foreach (var item in orderbooksList)
                    {
                        cartTotal += item.Quantity;
                    }

                    ViewData["action"] = "Detail/"+ orderid;
                    ViewBag.totalPrice = orders.TotalPrice;
                    ViewBag.totalCount = totalCount;
                    ViewBag.cartTotal = cartTotal;
                    ViewBag.totalPage = Math.Ceiling(totalCount * 1.0 / pageSize);

                    // 计算跳过的数据量
                    int skip = (pageIndex - 1) * pageSize;

                    // 构建基础查询
                    IQueryable<Orderbooks> query = orderbooksList.AsQueryable().Include(u => u.Book);

                    // 分页查询
                    List<Orderbooks> items = query
                        .Skip(skip)
                        .Take(pageSize)
                        .ToList();
                    return View(items);
                }
                ViewData["action"] = "Detail";
            }
            return View();
        }
        /// <summary>
        /// 生成订单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateOrder()
        {
            OrdersService ordersService = new OrdersService(_orderRepository);
            OrderbooksService orderbooksService = new OrderbooksService(_orderbooksRepository);
            ISession session = HttpContext.Session;
            ReturnData<Orders> result = new ReturnData<Orders>(0);
            //用户信息失效，需要重新登录
            if (string.IsNullOrEmpty(session.GetString("Users")))
            {
                result.IntResult = 0;
                return Json(result);
            }
            else
            {
                if (!string.IsNullOrEmpty(session.GetString("ShopCarts")))
                {
                    List<ShopCarts>? shopCartsList = JsonConvert.DeserializeObject<List<ShopCarts>>(session.GetString("ShopCarts"));
                    Users user = JsonConvert.DeserializeObject<Users>(session.GetString("Users"));
                    if (shopCartsList.Count > 0)
                    {
                        //生成订单
                        Orders orders = new Orders();
                        orders.UserId = user.UserId;
                        orders.OrderDate = DateTime.Now;
                        orders.TotalPrice = shopCartsList.Sum(u => u.TotalPrice);
                        orders.OrderState = 0;
                        List<Orderbooks> orderbooksList = new List<Orderbooks>();

                        //生成订单明细
                        foreach (var item in shopCartsList)
                        {
                            Orderbooks orderbooks = new Orderbooks();
                            orderbooks.OrderId = orders.OrderId;
                            orderbooks.BookId = item.BookId;
                            orderbooks.UnitPrice = item.UnitPrice;
                            orderbooks.Quantity = item.Quantity;
                            orderbooksList.Add(orderbooks);
                        }
                        result = await ordersService.Insert(orders, orderbooksList);
                        //清空购物车
                        session.Remove("ShopCarts");
                        result.IntResult = 1;
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
                    result.IntResult = 0;
                    return Json(result);
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int OrderID)
        {
            OrdersService ordersService = new OrdersService(_orderRepository);
            ISession session = HttpContext.Session;
            ReturnData<Orders> result = new ReturnData<Orders>(0);
            //用户信息失效，需要重新登录
            if (string.IsNullOrEmpty(session.GetString("Users")))
            {
                result.IntResult = 0;
                return Json(result);
            }
            else
            {
                result = await ordersService.Delete(OrderID);
                return Json(result);
            }
        }
        public IActionResult UsersIndex(int pageIndex = 1)
        {
            int pageSize = 15;
            int totalCount = 0;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            //读取Session
            ISession session = HttpContext.Session;
            if (!string.IsNullOrEmpty(session.GetString("Users")))
            {
                Users user = JsonConvert.DeserializeObject<Users>(session.GetString("Users"));
                ViewBag.Users = user;

                OrdersService modelService = new OrdersService(_orderRepository);
                IQueryable<Orders> items = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.UserId == user.UserId, o => o.OrderDate, false);
                ViewBag.totalCount = totalCount;
                ViewBag.totalPage = Math.Ceiling(totalCount * 1.0 / pageSize);
                return View(items);
            }
            else
            {
                return View();
            }
        }
        #endregion

        #region 管理员使用
        // 管理员后台订单管理主视图
        public IActionResult Index()
        {
            return View();
        }
        // 通过WebAPI获取订单列表
        public async Task<string> GetOrderList(int page, int rows, string searname)
        {
            string sear = string.IsNullOrEmpty(searname) ? "" : "," + searname;
            string strParam = page + "," + rows + sear;
            string apiurl = _apiUrl + "/api/Orders/GetOrderList/" + strParam;
            var response = await _httpClient.GetAsync(apiurl);
            return await response.Content.ReadAsStringAsync();

        }

        [HttpPost]
        public async Task<string> Edit(int id, [Bind("OrderId,OrderState")] Orders model)
        {
            OrdersService modelService = new OrdersService(_orderRepository);
            ReturnData<Orders> returnData = new ReturnData<Orders>(0);
            if (id != model.OrderId)
            {
                returnData.IntResult = -1;
                return Json(returnData).ToString();
            }
            string strParam = id + "," + model.OrderState;
            string apiurl = _apiUrl + "/api/Orders/Edit/" + strParam;
            var response = await _httpClient.GetAsync(apiurl);
            return response.Content.ReadAsStringAsync().Result;
        }
        [HttpPost]
        public async Task<string> Delete(string strId)
        {
            string apiurl = _apiUrl + "/api/Orders/Delete/" + strId;
            var response = await _httpClient.DeleteAsync(apiurl);
            return response.Content.ReadAsStringAsync().Result;
        }
        #endregion
    }
}
