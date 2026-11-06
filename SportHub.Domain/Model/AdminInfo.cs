using System;
using System.Collections.Generic;
using System.Net;

namespace SportHub.Domain.Model;

/// <summary>
/// 管理员
/// </summary>
public partial class AdminInfo
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 别名
    /// </summary>
    public string? Alias { get; set; }

    /// <summary>
    /// 登录名
    /// </summary>
    public string? LoginName { get; set; }

    /// <summary>
    /// 访问ip白名单
    /// </summary>
    public List<string>? HostWhiteList { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public long UpdatedTime { get; set; }
    public bool Disabled { get; set; }
}
