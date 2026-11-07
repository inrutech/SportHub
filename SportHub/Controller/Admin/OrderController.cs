using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request.Order;
using SportHub.Service.Admin;

using Zero.Core.Result;

namespace SportHub.Controller.Admin;

public class OrderController(OrderService service) : BaseController
{
    [HttpPost("SyncOrder")]
    public Task<SysResult<bool>> SyncOrderAsync(SyncOrderRequest request)
    {
        return service.SyncOrderAsync(request);
    }
}