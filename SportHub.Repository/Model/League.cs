using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class League
{
    public long Id { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// 联赛代码枚举（LEAGUE/CUP/SUPER/INTERNATIONAL）
    /// </summary>
    public long? LeagueCode { get; set; }

    public string? AliasArray { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
