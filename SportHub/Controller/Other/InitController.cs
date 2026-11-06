using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Other.Request;
using SportHub.Service.Other;
using SportHub.Domain.Client.VIC.Response;

using Zero.Core.Result;

namespace SportHub.Controller.Other;

public class InitController(InitService service) : BaseController
{

    [HttpPost("SyncLeague")]
    public Task<SysResult<bool>> SyncLeagueAsync(SyncLeagueRequest request)
    {
        return service.SyncLeagueAsync(request);
    }


    [HttpPost("GetOrderDetail")]
    public Task<SysResult<GetOrderDetailResponse>> GetOrderDetailAsync(GetOrderDetailRequest request)
    {
        return service.GetOrderDetailAsync(request);
    }
}