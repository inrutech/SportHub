using System;
using SportHub.Core.Code;
using SportHub.Domain.Admin.Response.Config;
using SportHub.Filter;
using SportHub.Service.Admin;
using Microsoft.AspNetCore.Mvc;
using Zero.Core.Result;
using SportHub.Controller.Admin;
using SportHub.Domain.Admin.Request.Config;

namespace SportHub.Controller.Admin;

/// <summary>
/// 配置控制器
/// </summary>
/// <param name="service"></param>
public class ConfigController(ConfigService service) : BaseController
{
    /// <summary>
    ///     获取分页列表
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AdminValid(grantSystemCode: GrantSystemCode.Config)]
    [HttpGet("List")]
    public Task<SysResult<List<ListResponse>>> ListAsync([FromQuery] ListRequest request)
    {
        return service.ListAsync(request);
    }

    /// <summary>
    /// 设置配置
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AdminValid(grantSystemCode: GrantSystemCode.Config)]
    [HttpPost("Set")]
    public Task<SysResult<bool>> SetAsync(SetRequest request)
    {
        return service.SetAsync(request);
    }

}
