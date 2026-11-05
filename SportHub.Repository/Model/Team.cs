using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class Team
{
    public long Id { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }

    /// <summary>
    /// 联盟编号
    /// </summary>
    public long? LeagueId { get; set; }

    public string? NameEn { get; set; }

    public string? NameZhCn { get; set; }

    public string? NameZhTw { get; set; }

    public string? SourceArrayJson { get; set; }
}
