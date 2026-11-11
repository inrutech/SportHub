using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request.Admin;
using SportHub.Domain.Admin.Response.Admin;
using SportHub.Filter;
using SportHub.Service.Admin;

using Zero.Core.Result;
using Zero.Core.Web;
using Zero.Core.Web.Request;

namespace SportHub.Controller.Admin;

public class AdminController(AdminService service) : BaseController
{

    [HttpGet("Page")]
    public Task<SysResult<PageList<AdminPageResponse>>> PageAsync([FromQuery] AdminPageRequest request)
    {
        return service.PageAsync(request);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AdminValid]
    [HttpPost("ResetPassword")]
    public Task<SysResult<bool>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return service.ResetPasswordAsync(request);
    }

    /// <summary>
    /// 获取新的验谷歌验证密钥
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AdminValid]
    [HttpGet("GetGoogleAuthSecret")]
    public Task<SysResult<GetGoogleAuthSecretResponse>> GetGoogleAuthSecretAsync([FromQuery] GetGoogleAuthSecretRequest request)
    {
        return service.GetGoogleAuthSecretAsync(request);
    }

    /// <summary>
    /// 设置谷歌验证密钥
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AdminValid]
    [HttpPost("SetGoogleAuthSecret")]
    public Task<SysResult<bool>> SetGoogleAuthSecretAsync(SetGoogleAuthSecretRequest request)
    {
        return service.SetGoogleAuthSecretAsync(request);
    }

    /// <summary>
    /// 设置管理员信息
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AdminValid]
    [HttpPost("Set")]
    public Task<SysResult<bool>> SetAsync(SetRequest request)
    {
        return service.SetAsync(request);
    }
}