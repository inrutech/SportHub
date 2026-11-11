using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class Task
{
    public long Id { get; set; }

    public List<long>? OrderId { get; set; }

    public long? OrderLegIdList { get; set; }

    /// <summary>
    /// 目标平台
    /// </summary>
    public long? TargetSourceCode { get; set; }

    /// <summary>
    /// 倍数,,如果方向则选择负数
    /// </summary>
    public decimal? Ratio { get; set; }

    public long? ProcessCode { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
