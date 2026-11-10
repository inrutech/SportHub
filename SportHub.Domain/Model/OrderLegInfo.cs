using SportHub.Core.Code;

namespace SportHub.Domain.Model;

/// <summary>
/// 订单腿
/// </summary>
public partial class OrderLegInfo
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public long? OrderId { get; set; }

    /// <summary>
    /// 串关中的第几场比赛序号
    /// </summary>
    public long? LegIndex { get; set; }

    /// <summary>
    /// 联赛编号
    /// </summary>
    public long? LeagueId { get; set; }

    /// <summary>
    /// 主队团队编号
    /// </summary>
    public long? HomeTeamId { get; set; }

    /// <summary>
    /// 客队团队编号
    /// </summary>
    public long? AwayTeamId { get; set; }

    /// <summary>
    /// 比赛阶段
    /// </summary>
    public PeriodCode? PeriodCode { get; set; }

    /// <summary>
    /// 玩法类型
    /// </summary>
    public MarketCode? MarketCode { get; set; }

    /// <summary>
    /// 盘口数值
    /// </summary>
    public string? HandicapValue { get; set; }

    /// <summary>
    /// 投注选项
    /// </summary>
    public SelectionCode? SelectionCode { get; set; }

    /// <summary>
    /// 该腿下单时原始赔率
    /// </summary>
    public decimal? BetOdds { get; set; }

    /// <summary>
    /// 实际结算有效赔率
    /// </summary>
    public decimal? SettledOdds { get; set; }

    /// <summary>
    /// 结算状态
    /// </summary>
    public LegSettleCode? LegSettleCode { get; set; }

    /// <summary>
    /// 源编号
    /// </summary>
    public long? SourceNo { get; set; }

    /// <summary>
    /// 比赛编号
    /// </summary>
    public long? MatchId { get; set; }

    /// <summary>
    /// 比赛开始时间
    /// </summary>
    public long? MatchTime { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Describe { get; set; }

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
