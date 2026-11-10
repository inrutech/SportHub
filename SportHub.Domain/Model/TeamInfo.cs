namespace SportHub.Domain.Model;

/// <summary>
/// 球队
/// </summary>
public partial class TeamInfo
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 联赛编号
    /// </summary>
    public long? LeagueId { get; set; }

    /// <summary>
    /// 球队名称(英文)
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// 球队名称(简体中文)
    /// </summary>
    public string? NameZhCn { get; set; }

    /// <summary>
    /// 球队名称(繁体中文)
    /// </summary>
    public string? NameZhTw { get; set; }

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
