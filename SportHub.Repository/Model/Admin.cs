using System;
using System.Collections.Generic;
using System.Net;

namespace SportHub.Repository.Model;

public partial class Admin
{
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
    /// 密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 谷歌授权码,关键操作使用
    /// </summary>
    public string? GoogleAuthSecret { get; set; }

    /// <summary>
    /// 访问ip白名单
    /// </summary>
    public List<IPAddress>? HostWhiteList { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
