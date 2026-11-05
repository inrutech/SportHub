using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class League
{
    public long Id { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// 联赛缩写（如 EPL、UCL）
    /// </summary>
    public string? Abbr { get; set; }

    public string? Country { get; set; }

    /// <summary>
    /// 国家代码（ISO/FIFA，可空）
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// 区域/洲别（如 Europe、Asia）
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// 联赛代码枚举（LEAGUE/CUP/SUPER/INTERNATIONAL）
    /// </summary>
    public long? LeagueCode { get; set; }

    /// <summary>
    /// 扩展元数据（jsonb）
    /// </summary>
    public string? MetaJson { get; set; }

    public string? AliasArray { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
