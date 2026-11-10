
using SportHub.Domain.Model;
using SportHub.Repository.Model;

namespace SportHub.Domain.Admin.Response.Order;

public class OrderListResponse
{
    public OrderInfo? OrderInfo { get; set; }

    public OrderLegInfo? OrderLegInfo { get; set; }

    public LeagueInfo? LeagueInfo { get; set; }

    public MatchInfo? MatchInfo { get; set; }

    public TeamInfo? HomeTeamInfo { get; set; }

    public TeamInfo? AwayTeamInfo { get; set; }
}