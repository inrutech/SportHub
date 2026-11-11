using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request.My;
using SportHub.Domain.Admin.Response.My;
using SportHub.Service.Admin;

using Zero.Core.Result;

namespace SportHub.Controller.Admin;

public class MyController(MyService service) : BaseController
{
    // [HttpPost("SetToken")]
    // public Task<SysResult<bool>> SetTokenAsync(SetTokenRequest request)
    // {
    //     return service.SetTokenAsync(request);
    // }

    /// <summary>
    /// 管理员登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    public Task<SysResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        return service.LoginAsync(request);
    }
}