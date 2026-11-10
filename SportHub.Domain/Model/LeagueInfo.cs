using SportHub.Core.Code;

namespace SportHub.Domain.Model;

/// <summary>
/// 联赛
/// </summary>
public partial class LeagueInfo
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 联赛类型
    /// </summary>
    public LeagueCode? LeagueCode { get; set; }

    /// <summary>
    /// 国家编号
    /// </summary>
    public long? CountryId { get; set; }

    /// <summary>
    /// 国家(英文)
    /// </summary>
    public string? CountryEn { get; set; }

    /// <summary>
    /// 国家(简体中文)
    /// </summary>
    public string? CountryZhCn { get; set; }

    /// <summary>
    /// 国家(繁体中文)
    /// </summary>
    public string? CountryZhTw { get; set; }

    /// <summary>
    /// 联赛名称(英文)
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// 联赛名称(简体中文)
    /// </summary>
    public string? NameZhCn { get; set; }

    /// <summary>
    /// 联赛名称(繁体中文)
    /// </summary>
    public string? NameZhTw { get; set; }

    /// <summary>
    /// 联赛简称(英文)
    /// </summary>
    public string? ShortNameEn { get; set; }

    /// <summary>
    /// 联赛简称(简体中文)
    /// </summary>
    public string? ShortNameZhCn { get; set; }

    /// <summary>
    /// 联赛简称(繁体中文)
    /// </summary>
    public string? ShortNameZhTw { get; set; }

    /// <summary>
    /// 数据源映射
    /// </summary>
    public Dictionary<long, string>? SourceArray { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public long UpdatedTime { get; set; }

    /// <summary>
    /// 是否禁用
    /// </summary>
    public bool Disabled { get; set; }
}
