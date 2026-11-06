using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

/// <summary>
/// 配置
/// </summary>
public partial class Config
{
    public long Id { get; set; }

    /// <summary>
    /// 配置类型
    /// </summary>
    public long? ConfigCode { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public long? DataCode { get; set; }

    /// <summary>
    /// 全局hash健
    /// </summary>
    public long? GlobalHash { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public long? Sort { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
