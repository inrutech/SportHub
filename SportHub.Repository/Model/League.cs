using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class League
{
    public long Id { get; set; }

    /// <summary>
    /// 联赛代码枚举（LEAGUE/CUP/SUPER/INTERNATIONAL）
    /// </summary>
    public long? LeagueCode { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }

    public string? NameEn { get; set; }

    public string? NameZhCn { get; set; }

    public string? NameZhTw { get; set; }

    public string? ShortNameEn { get; set; }

    public string? ShortNameZhCn { get; set; }

    public string? ShortNameZhTw { get; set; }

    public long? CountryId { get; set; }

    public string? CountryEn { get; set; }

    public string? CountryZhCn { get; set; }

    public string? CountryZhTw { get; set; }

    public string? SourceArrayJson { get; set; }
}
