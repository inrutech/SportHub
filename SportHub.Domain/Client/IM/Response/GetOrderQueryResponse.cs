using System.Text.Json.Serialization;

namespace SportHub.Domain.Client.IM.Response;

public partial class GetOrderQueryResponse
{
    [JsonPropertyName("code")]
    public long Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public Data Data { get; set; }
}

public partial class Data
{
    [JsonPropertyName("page")]
    public long Page { get; set; }

    [JsonPropertyName("page_size")]
    public long PageSize { get; set; }

    [JsonPropertyName("total")]
    public long Total { get; set; }

    [JsonPropertyName("total_page")]
    public long TotalPage { get; set; }

    [JsonPropertyName("row_count")]
    public long RowCount { get; set; }

    [JsonPropertyName("start_time")]
    public string StartTime { get; set; }

    [JsonPropertyName("end_time")]
    public string EndTime { get; set; }

    [JsonPropertyName("settled")]
    public long Settled { get; set; }

    [JsonPropertyName("rows")]
    public List<Row> Rows { get; set; }
}

public partial class Row
{
    [JsonPropertyName("orderid")]
    public string Orderid { get; set; }

    [JsonPropertyName("bet_id")]
    public long BetId { get; set; }

    [JsonPropertyName("row_type")]
    public string RowType { get; set; }

    [JsonPropertyName("is_parlay")]
    public long IsParlay { get; set; }

    [JsonPropertyName("leg_no")]
    public long LegNo { get; set; }

    [JsonPropertyName("leg_count")]
    public long LegCount { get; set; }

    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [JsonPropertyName("user_name")]
    public string UserName { get; set; }

    [JsonPropertyName("store_id")]
    public long StoreId { get; set; }

    [JsonPropertyName("store_name")]
    public string StoreName { get; set; }

    [JsonPropertyName("shop_id")]
    public long ShopId { get; set; }

    [JsonPropertyName("shop_name")]
    public string ShopName { get; set; }

    [JsonPropertyName("bet_type")]
    public string BetType { get; set; }

    [JsonPropertyName("bet_type_name")]
    public string BetTypeName { get; set; }

    [JsonPropertyName("period")]
    public string Period { get; set; }

    [JsonPropertyName("is_rolling")]
    public long IsRolling { get; set; }

    [JsonPropertyName("match_id")]
    public long MatchId { get; set; }

    [JsonPropertyName("league_chs")]
    public string LeagueChs { get; set; }

    [JsonPropertyName("home_chs")]
    public string HomeChs { get; set; }

    [JsonPropertyName("away_chs")]
    public string AwayChs { get; set; }

    [JsonPropertyName("market_title")]
    public string MarketTitle { get; set; }

    [JsonPropertyName("line_display")]
    public string LineDisplay { get; set; }

    [JsonPropertyName("selection_display")]
    public string SelectionDisplay { get; set; }

    [JsonPropertyName("side")]
    public string Side { get; set; }

    [JsonPropertyName("pick_label")]
    public string PickLabel { get; set; }

    [JsonPropertyName("order_odds")]
    public string OrderOdds { get; set; }

    [JsonPropertyName("leg_odds")]
    public string? LegOdds { get; set; }

    [JsonPropertyName("stake")]
    public string Stake { get; set; }

    [JsonPropertyName("potential_return")]
    public string PotentialReturn { get; set; }

    [JsonPropertyName("payout")]
    public string Payout { get; set; }

    [JsonPropertyName("payout_net")]
    public string PayoutNet { get; set; }

    [JsonPropertyName("win_loss")]
    public string WinLoss { get; set; }

    [JsonPropertyName("settle_status")]
    public long SettleStatus { get; set; }

    [JsonPropertyName("settle_status_name")]
    public string SettleStatusName { get; set; }

    [JsonPropertyName("money_status")]
    public long MoneyStatus { get; set; }

    [JsonPropertyName("money_status_name")]
    public string MoneyStatusName { get; set; }

    [JsonPropertyName("bet_time")]
    public string BetTime { get; set; }

    [JsonPropertyName("bet_ts")]
    public long BetTs { get; set; }

    [JsonPropertyName("settle_time")]
    public string SettleTime { get; set; }

    [JsonPropertyName("settle_ts")]
    public long SettleTs { get; set; }

    [JsonPropertyName("bet_ip")]
    public string BetIp { get; set; }

    [JsonPropertyName("match_time")]
    public string? MatchTime { get; set; }
}