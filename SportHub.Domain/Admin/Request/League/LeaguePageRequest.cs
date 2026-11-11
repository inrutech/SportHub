using Zero.Core.Web.Request;

namespace SportHub.Domain.Admin.Request.League;

public class LeaguePageRequest : PageRequest
{
    public long? Id { get; set; }

    public string? NameEn { get; set; }

    public string? NameZhCn { get; set; }
}
