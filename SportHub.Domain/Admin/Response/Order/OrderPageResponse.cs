using SportHub.Domain.Model;

namespace SportHub.Domain.Admin.Response.Order;

public class OrderPageResponse
{
    public OrderInfo? OrderInfo { get; set; }
    public List<OrderLegInfo?> OrderLegList { get; set; }
    public List<LeagueInfo?> LeagueList { get; set; }
    public List<MatchInfo?> MatchList { get; set; }
    public List<TeamInfo?> TeamList { get; set; }
}
