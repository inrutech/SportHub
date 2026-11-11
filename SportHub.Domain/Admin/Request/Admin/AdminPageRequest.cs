using System;

using Zero.Core.Web.Request;

namespace SportHub.Domain.Admin.Request.Admin;

/// <summary>
/// 管理员分页页面请求
/// </summary>
public class AdminPageRequest : PageRequest
{
    /// <summary>
    /// id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 别名
    /// </summary>
    public string? Alias { get; set; }

    /// <summary>
    ///登录名
    /// </summary>
    public string? LoginName { get; set; }


}
