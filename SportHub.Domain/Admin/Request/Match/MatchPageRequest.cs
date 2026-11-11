using Zero.Core.Web.Request;

namespace SportHub.Domain.Admin.Request.Match;

public class MatchPageRequest : PageRequest
{
    public long? Id { get; set; }

    public long? LeagueId { get; set; }

    public long? HomeTeamId { get; set; }

    public long? AwayTeamId { get; set; }
}
