using System.Text.Json.Serialization;

namespace SportHub.Domain.Client.VIC.Response;

public partial class GetOrderDetailResponse
{
    [JsonPropertyName("code")]
    public long Code { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("msg")]
    public string? Msg { get; set; }

    [JsonPropertyName("__currentServiceId__")]
    public string? CurrentServiceId { get; set; }

    [JsonPropertyName("info")]
    public List<OrderDetailInfo>? Info { get; set; }

    [JsonPropertyName("odds")]
    public List<OrderDetailOdds>? Odds { get; set; }

    [JsonPropertyName("schedule")]
    public List<OrderDetailSchedule>? Schedule { get; set; }
}

public partial class OrderDetailInfo
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("orderid")]
    public string? Orderid { get; set; }

    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    [JsonPropertyName("shop_id")]
    public string? ShopId { get; set; }

    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    [JsonPropertyName("stake")]
    public string? Stake { get; set; }

    [JsonPropertyName("odds")]
    public string? Odds { get; set; }

    [JsonPropertyName("potential_return")]
    public string? PotentialReturn { get; set; }

    [JsonPropertyName("bet_type")]
    public string? BetType { get; set; }

    [JsonPropertyName("league_chs")]
    public string? LeagueChs { get; set; }

    [JsonPropertyName("home_chs")]
    public string? HomeChs { get; set; }

    [JsonPropertyName("away_chs")]
    public string? AwayChs { get; set; }

    [JsonPropertyName("market_title")]
    public string? MarketTitle { get; set; }

    [JsonPropertyName("selection_display")]
    public string? SelectionDisplay { get; set; }

    [JsonPropertyName("bet_json")]
    public string? BetJson { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("settle_status")]
    public string? SettleStatus { get; set; }

    [JsonPropertyName("payout")]
    public string? Payout { get; set; }

    [JsonPropertyName("money_status")]
    public string? MoneyStatus { get; set; }

    [JsonPropertyName("createtime")]
    public string? Createtime { get; set; }

    [JsonPropertyName("updatetime")]
    public string? Updatetime { get; set; }

    [JsonPropertyName("bet_unlock_time")]
    public string? BetUnlockTime { get; set; }

    [JsonPropertyName("ip")]
    public string? Ip { get; set; }
}

public partial class OrderDetailOdds
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    [JsonPropertyName("league_chs")]
    public string? LeagueChs { get; set; }

    [JsonPropertyName("home_chs")]
    public string? HomeChs { get; set; }

    [JsonPropertyName("away_chs")]
    public string? AwayChs { get; set; }

    [JsonPropertyName("home_score")]
    public string? HomeScore { get; set; }

    [JsonPropertyName("away_score")]
    public string? AwayScore { get; set; }

    [JsonPropertyName("handicap")]
    public string? Handicap { get; set; }

    [JsonPropertyName("europe_odds")]
    public string? EuropeOdds { get; set; }

    [JsonPropertyName("over_under")]
    public string? OverUnder { get; set; }
}

public partial class OrderDetailSchedule
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    [JsonPropertyName("league_chs_short")]
    public string? LeagueChsShort { get; set; }

    [JsonPropertyName("home_chs")]
    public string? HomeChs { get; set; }

    [JsonPropertyName("away_chs")]
    public string? AwayChs { get; set; }

    [JsonPropertyName("match_time")]
    public string? MatchTime { get; set; }

    [JsonPropertyName("home_score")]
    public string? HomeScore { get; set; }

    [JsonPropertyName("away_score")]
    public string? AwayScore { get; set; }
}
