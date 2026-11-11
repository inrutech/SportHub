using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request.Order;
using SportHub.Domain.Admin.Response.Order;
using SportHub.Service.Admin;

using Zero.Core.Result;
using Zero.Core.Web;
using Zero.Core.Web.Request;

namespace SportHub.Controller.Admin;

public class OrderController(OrderService service) : BaseController
{
    [HttpGet("Page")]
    public Task<SysResult<PageList<OrderPageResponse>>> PageAsync([FromQuery] OrderPageRequest request)
    {
        return service.PageAsync(request);
    }

    [HttpPost("SyncOrder")]
    public Task<SysResult<SyncOrderResponse>> SyncOrderAsync(SyncOrderRequest request)
    {
        return service.SyncOrderAsync(request);
    }

    [HttpGet("OrderList")]
    public Task<SysResult<List<OrderListResponse>>> OrderListAsync([FromQuery] OrderListRequest request)
    {
        return service.OrderListAsync(request);
    }
}