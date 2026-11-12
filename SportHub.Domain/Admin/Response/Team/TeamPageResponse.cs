using SportHub.Domain.Model;

namespace SportHub.Domain.Admin.Response.Team;

public class TeamPageResponse
{
    public TeamInfo? TeamInfo { get; set; }
    public LeagueInfo? LeagueInfo { get; set; }
}
