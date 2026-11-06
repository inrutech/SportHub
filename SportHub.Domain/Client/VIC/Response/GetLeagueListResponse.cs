
using System.Text.Json.Serialization;

namespace SportHub.Domain.Client.VIC.Response;

public partial class GetLeagueListResponse
{
    [JsonPropertyName("code")]
    public long Code { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("__currentServiceId__")]
    public string CurrentServiceId { get; set; }

    [JsonPropertyName("listDataTotal")]
    public ListDataTotal ListDataTotal { get; set; }

    [JsonPropertyName("listData")]
    public List<ListDatum> ListData { get; set; }
}

public partial class ListDatum
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("league_id")]
    public long LeagueId { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonPropertyName("name_en")]
    public string NameEn { get; set; }

    [JsonPropertyName("name_en_short")]
    public string NameEnShort { get; set; }

    [JsonPropertyName("name_chs")]
    public string NameChs { get; set; }

    [JsonPropertyName("name_chs_short")]
    public string NameChsShort { get; set; }

    [JsonPropertyName("name_cht")]
    public string NameCht { get; set; }

    [JsonPropertyName("name_cht_short")]
    public string NameChtShort { get; set; }

    [JsonPropertyName("type")]
    public long Type { get; set; }

    [JsonPropertyName("sub_sclass_en")]
    public string SubSclassEn { get; set; }

    [JsonPropertyName("sub_sclass_cn")]
    public string SubSclassCn { get; set; }

    [JsonPropertyName("sum_round")]
    public string SumRound { get; set; }

    [JsonPropertyName("curr_round")]
    public string CurrRound { get; set; }

    [JsonPropertyName("curr_season")]
    public string CurrSeason { get; set; }

    [JsonPropertyName("country_id")]
    public long CountryId { get; set; }

    [JsonPropertyName("country_en")]
    public string CountryEn { get; set; }

    [JsonPropertyName("country_cn")]
    public string CountryCn { get; set; }

    [JsonPropertyName("league_logo")]
    public string LeagueLogo { get; set; }

    [JsonPropertyName("country_logo")]
    public string CountryLogo { get; set; }

    [JsonPropertyName("area_id")]
    public long AreaId { get; set; }

    [JsonPropertyName("is_disabled")]
    public long IsDisabled { get; set; }

    [JsonPropertyName("is_major")]
    public long IsMajor { get; set; }

    [JsonPropertyName("rule")]
    public string Rule { get; set; }

    [JsonPropertyName("rule_en")]
    public string RuleEn { get; set; }

    [JsonPropertyName("createtime")]

    public long Createtime { get; set; }

    [JsonPropertyName("updatetime")]

    public long Updatetime { get; set; }

    [JsonPropertyName("weigh")]

    public long Weigh { get; set; }

    [JsonPropertyName("keyid")]

    public long Keyid { get; set; }

    [JsonPropertyName("createtime_name")]
    public string CreatetimeName { get; set; }

    [JsonPropertyName("updatetime_name")]
    public string UpdatetimeName { get; set; }

    [JsonPropertyName("type_name")]
    public string TypeName { get; set; }
}

public partial class ListDataTotal
{
    [JsonPropertyName("id")]

    public long Id { get; set; }

    [JsonPropertyName("_count_")]

    public long Count { get; set; }
}