using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class Order
{
    public long Id { get; set; }

    /// <summary>
    /// 源订单号
    /// </summary>
    public string? SourceNo { get; set; }

    /// <summary>
    /// 订单类型
    /// </summary>
    public long? OrderCode { get; set; }

    /// <summary>
    /// 总关数
    /// </summary>
    public long? LegCount { get; set; }

    /// <summary>
    /// 下注总数
    /// </summary>
    public decimal? Stake { get; set; }

    /// <summary>
    /// 理论总赔率（各腿赔率连乘）
    /// </summary>
    public decimal? Odds { get; set; }

    /// <summary>
    /// 最高理论派彩金额（本金 * 总赔率)
    /// </summary>
    public decimal? PotentialPayout { get; set; }

    /// <summary>
    /// 实际结算派彩金额
    /// </summary>
    public decimal? ActualPayout { get; set; }

    /// <summary>
    /// 订单结算状态
    /// </summary>
    public long? OrderSettleCode { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
