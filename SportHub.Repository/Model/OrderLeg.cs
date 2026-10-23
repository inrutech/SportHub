using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class OrderLeg
{
    public long Id { get; set; }

    public long? OrderId { get; set; }

    /// <summary>
    /// 源编号
    /// </summary>
    public long? SourceCode { get; set; }

    /// <summary>
    /// 串关中的第几场比赛序号
    /// </summary>
    public long? LegIndex { get; set; }

    /// <summary>
    /// 联盟编号
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
    /// 比赛开始时间
    /// </summary>
    public long? MatchStartTime { get; set; }

    /// <summary>
    /// 比赛阶段（FULL_TIME:全场, HALF_TIME:上半场）
    /// </summary>
    public long? PeriodCode { get; set; }

    /// <summary>
    /// 玩法类型（ASIAN_HANDICAP:让球, OVER_UNDER:大小球, MONEYLINE_1X2:独赢）
    /// </summary>
    public long? MarketCode { get; set; }

    /// <summary>
    /// 盘口数值（如大小球 1.25, 1.5, 1.75 等；独赢为空）
    /// </summary>
    public decimal? HandicapValue { get; set; }

    /// <summary>
    /// 投注选项（如 Under, Over, HOME, AWAY, DRAW 等）
    /// </summary>
    public long? SelectionCode { get; set; }

    /// <summary>
    /// 下注时的比赛即时比分
    /// </summary>
    public long? BetScore { get; set; }

    /// <summary>
    /// 比赛最终半场比分
    /// </summary>
    public long? HalfTimeScore { get; set; }

    /// <summary>
    /// 比赛最终全场比分
    /// </summary>
    public long? FullTimeScore { get; set; }

    /// <summary>
    /// 该腿下单时的原始赔率
    /// </summary>
    public decimal? BetOdds { get; set; }

    /// <summary>
    /// 实际结算有效赔率（全赢为原始赔率，赢一半为折算赔率，走水为1.0等）
    /// </summary>
    public decimal? SettledOdds { get; set; }

    /// <summary>
    /// 结算状态（PENDING:待定, WON:全赢, HALF_WON:赢一半, PUSH:走水, HALF_LOST:输一半, LOST:全输）
    /// </summary>
    public long? LegSettleCode { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
