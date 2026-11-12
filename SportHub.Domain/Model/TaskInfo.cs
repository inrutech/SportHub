using SportHub.Core.Code;

namespace SportHub.Domain.Model;

/// <summary>
/// 任务
/// </summary>
public partial class TaskInfo
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
    /// 订单腿编号集合
    /// </summary>
    public List<long>? OrderLegIdList { get; set; }

    /// <summary>
    /// 目标平台
    /// </summary>
    public SourceCode? TargetSourceCode { get; set; }

    /// <summary>
    /// 倍数，若方向则为负数
    /// </summary>
    public decimal? Ratio { get; set; }

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
    public ProcessCode ProcessCode { get; set; }
    public string? Result { get; set; }
}
