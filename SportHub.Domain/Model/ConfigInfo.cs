using SportHub.Core.Code;

using System;
using System.Collections.Generic;

namespace SportHub.Domain.Model;

/// <summary>
/// 配置
/// </summary>
public partial class ConfigInfo
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 配置类型
    /// </summary>
    public ConfigCode? ConfigCode { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public DataCode? DataCode { get; set; }

    /// <summary>
    /// 全局hash健
    /// </summary>
    public long? GlabalHash { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public long? Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public long UpdatedTime { get; set; }


    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Disabled { get; set; }
}
