using SportHub.Domain.Model;

namespace SportHub.Domain.Admin.Response.Match;

public class MatchPageResponse
{
    public MatchInfo? MatchInfo { get; set; }
    public LeagueInfo? LeagueInfo { get; set; }
    public TeamInfo? HomeTeamInfo { get; set; }
    public TeamInfo? AwayTeamInfo { get; set; }
}
