namespace SportHub.Domain.Model;

/// <summary>
/// 比赛
/// </summary>
public partial class MatchInfo
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
    /// 主队编号
    /// </summary>
    public long? HomeTeamId { get; set; }

    /// <summary>
    /// 客队编号
    /// </summary>
    public long? AwayTeamId { get; set; }

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
