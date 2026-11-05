using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class Team
{
    public long Id { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// 球队缩写（如 PSG、MU）
    /// </summary>
    public string? Abbr { get; set; }

    public string? Country { get; set; }

    /// <summary>
    /// 球队简称
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// 球队英文名
    /// </summary>
    public string? EnglishName { get; set; }

    /// <summary>
    /// 国家代码（ISO/FIFA，可空）
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// 所在城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 成立年份
    /// </summary>
    public long? FoundedYear { get; set; }

    /// <summary>
    /// 主场/球场
    /// </summary>
    public string? Stadium { get; set; }

    /// <summary>
    /// 扩展元数据（jsonb）
    /// </summary>
    public string? MetaJson { get; set; }

    public string? AliasArray { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
