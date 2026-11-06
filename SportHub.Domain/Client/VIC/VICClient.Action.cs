using Microsoft.Extensions.Logging;

using SportHub.Domain.Client.VIC.Response;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Zero.Core.Result;

namespace SportHub.Domain.Client.VIC;

public partial class VICClient
{
    public async Task<SysResult<GetLeagueListResponse>> GetLeagueListAsync(string token)
    {

        var args = new Dictionary<string, object?>
        {
            ["tabParam"] = "",
            ["refFormId"] = "fa_football_league",
            ["_t"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
        };

        var payload = new Dictionary<string, object?>
        {
            ["tabParam"] = "",
            ["filter"] = "",
            ["_pageParam_"] = "eyJfb2Zmc2V0XyI6MSwiX2xpbWl0XyI6NTAwMCwiX29yZGVyQnlfIjoiY3JlYXRldGltZV9uYW1lIGRlc2MiLCJfY2FsY0ZpZWxkXyI6ImNvdW50KGlkKSJ9"
        };

        var headers = new Dictionary<string, string>
        {
            // ["__userkeyid__"] = token
            ["Cookie"] = $"Authorization={token}",
        };

        var r = await PostAsync<GetLeagueListResponse>("site.vic88888.com", "/site/customize/fa_football_league/executeService/listData", args, payload, headers);
        return r;
    }

    public async Task<SysResult<GetOrderDetailResponse>> GetOrderDetailAsync(string token, string ordId, string? matchId = null)
    {
        var args = new Dictionary<string, object?>
        {
            ["tabParam"] = "",
            ["refFormId"] = "fa_football_bet_order",
            ["_t"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
        };

        var payload = new Dictionary<string, object?>
        {
            ["ordid"] = ordId,
            ["match_id"] = string.IsNullOrWhiteSpace(matchId) ? "2997771, 2997773, 3012344, 3108822, 2962730, 3019189, 3008323" : matchId
        };

        var headers = new Dictionary<string, string>
        {
            // ["__userkeyid__"] = token,
            ["Cookie"] = $"Authorization={token}",
        };

        return await PostAsync<GetOrderDetailResponse>(
            "site.vic88888.com",
            "/site/customize/fa_football_bet_order/executeService/get_json",
            args,
            payload,
            headers);
    }

}
