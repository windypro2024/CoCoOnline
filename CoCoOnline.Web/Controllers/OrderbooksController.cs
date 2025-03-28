using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoCoOnline.Web.Controllers
{
    public class OrderbooksController : Controller
    {
        private readonly OrderbooksRepository _orderbooksRepository;
        public OrderbooksController(OrderbooksRepository orderbooksRepository)
        {
            _orderbooksRepository = orderbooksRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region 后台管理使用
        // 用户状态管理主页获取状态列表
        public IActionResult GetOrderBookList(int page, int rows,int OrderId)
        {
            OrderbooksService modelService = new OrderbooksService(_orderbooksRepository);
            int pageIndex = page;
            int pageSize = rows;
            int totalCount;
            IQueryable<Orderbooks> modelList = null;
            modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.OrderId == OrderId, u => u.Id, true);
            
            var temp = from o in modelList
                       select new
                       {
                           Id = o.Id,
                           OrderId = o.OrderId,
                           BookId = o.BookId,
                           BookTitle = o.Book.BookTitle,
                           BookAuthor = o.Book.BookAuthor,
                           Publisher = o.Book.Publisher,
                           UnitPrice = o.UnitPrice,
                           Quantity = o.Quantity,
                           TotalPrice = o.UnitPrice* o.Quantity

                       };
            return Json(new { total = totalCount, rows = temp });
        }
        #endregion
    }
}
