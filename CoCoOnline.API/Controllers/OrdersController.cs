using CoCoOnline.Models;
using CoCoOnline.Repository;
using CoCoOnline.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CoCoOnline.Uitles;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoCoOnline.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersRepository _orderRepository;

        public OrdersController(OrdersRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        // 获取分页数据
        [HttpGet("{page},{rows}")]
        public object GetOrderList(int page, int rows,string? searname)
        {
            OrdersService modelService = new OrdersService(_orderRepository);
            int pageIndex = page;
            int pageSize = rows;
            string userName = searname;
            IQueryable<Orders> modelList = null;
            int totalCount = 0;
            if (string.IsNullOrEmpty(userName))
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.OrderId > 0, u => u.OrderId, true);
            }
            else
            {
                modelList = modelService.GetPagedList(pageIndex, pageSize, out totalCount, u => u.User.UserName.Contains(userName), u => u.OrderId, true);
            }
            var temp = from o in modelList
                       select new
                       {
                           OrderId = o.OrderId,
                           OrderDate = o.OrderDate,
                           UserId = o.UserId,
                           UserName = o.User.UserName,
                           TotalPrice = o.TotalPrice,
                           OrderState = o.OrderState
                       };
            return new JsonResult(new { total = totalCount, rows =temp });
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public Orders GetOrderList(int id)
        {
            OrdersService ordersService = new OrdersService(_orderRepository);
            Orders order = ordersService.GetList(u => u.OrderId == id).FirstOrDefault();
            return order;
        }

        // PUT api/<OrdersController>/5
        [HttpGet("{id},{value}")]
        public ReturnData<Orders?> Edit(int id, string value)
        {
            OrdersService ordersService = new OrdersService(_orderRepository);
            Orders order = GetOrderList(id);
            order.OrderState =Convert.ToInt32(value);
            return ordersService.Update(order);
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{ids}")]
        public Task<ReturnData<Orders>> Delete(string ids)
        {
            OrdersService ordersService = new OrdersService(_orderRepository);
            return ordersService.Delete(ids);
        }
    }
}
