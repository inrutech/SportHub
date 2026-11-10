using SportHub.Core.Code;

namespace SportHub.Domain.Model;

/// <summary>
/// 订单
/// </summary>
public partial class OrderInfo
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 源订单号
    /// </summary>
    public string? SourceNo { get; set; }

    /// <summary>
    /// 订单类型
    /// </summary>
    public OrderCode? OrderCode { get; set; }

    /// <summary>
    /// 总关数
    /// </summary>
    public long? LegCount { get; set; }

    /// <summary>
    /// 下注总数
    /// </summary>
    public decimal? Stake { get; set; }

    /// <summary>
    /// 理论总赔率
    /// </summary>
    public decimal? Odds { get; set; }

    /// <summary>
    /// 最高理论派彩金额
    /// </summary>
    public decimal? PotentialPayout { get; set; }

    /// <summary>
    /// 实际结算派彩金额
    /// </summary>
    public decimal? ActualPayout { get; set; }

    /// <summary>
    /// 订单结算状态
    /// </summary>
    public OrderSettleCode? OrderSettleCode { get; set; }

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
