using System.ComponentModel;

namespace SportHub.Core.Code;

/// <summary>
/// 公共枚举
/// </summary>
[Flags]
public enum CommonCode : long
{
    /// <summary>
    /// 无标识。
    /// </summary>
    None = 0,

    /// <summary>
    ///     删除
    /// </summary>
    [Description("删除")]
    Removed = 1L,

    /// <summary>
    ///     禁用
    /// </summary>
    [Description("禁用")]
    Disabled = 1L << 1,
}