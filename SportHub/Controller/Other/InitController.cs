using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Other.Request;
using SportHub.Service.Other;
using SportHub.Domain.Client.IM.Response;

using Zero.Core.Result;

namespace SportHub.Controller.Other;

public class InitController(InitService service) : BaseController
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isReset"></param>
    /// <returns></returns>
    [HttpPost("InitConfig")]
    public Task<SysResult<bool>> InitConfigAsync(bool isReset = false)
    {
        return service.InitConfigAsync(isReset);
    }

    [HttpPost("InitAdmin")]
    public Task<SysResult<bool>> InitAdminAsync()
    {
        return service.InitAdminAsync();
    }
}