using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class Match
{
    public long Id { get; set; }

    /// <summary>
    /// 联盟编号
    /// </summary>
    public long? LeagueId { get; set; }

    public long? HomeTeamId { get; set; }

    public long? AwayTeamId { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
