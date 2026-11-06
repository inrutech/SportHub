using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request;
using SportHub.Service.Admin;

using Zero.Core.Result;

namespace SportHub.Controller.Admin;

public class MyController(MyService service) : BaseController
{
    [HttpPost("SetToken")]
    public Task<SysResult<bool>> SetTokenAsync(SetTokenRequest request)
    {
        return service.SetTokenAsync(request);
    }
}