using Microsoft.Extensions.Logging;

using SportHub.Domain.Client.IM.Response;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Zero.Core.Result;

namespace SportHub.Domain.Client.IM;

public partial class IMClient
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
            ["__userkeyid__"] = token,
            ["Origin"] = "https://site.vic88888.com",
            ["Referer"] = "https://site.vic88888.com/site/customize/fa_football_league/view.do",
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/156.0.0.0 Safari/537.36 Edg/156.0.0.0"
        };

        var r = await PostAsync<GetLeagueListResponse>("site.vic88888.com", "/site/customize/fa_football_league/executeService/listData", args, payload, headers);
        return r;
    }

    public async Task<SysResult<GetOrderDetailResponse>> GetOrderDetailAsync(string token, string ordId, string? matchId = null)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new SysResult<GetOrderDetailResponse>
            {
                Code = ErrorCode.SYS_FAIL,
                ErrorDesc = "缺少 Token",
                Result = null
            };
        }

        if (string.IsNullOrWhiteSpace(ordId))
        {
            return new SysResult<GetOrderDetailResponse>
            {
                Code = ErrorCode.SYS_FAIL,
                ErrorDesc = "缺少订单号",
                Result = null
            };
        }

        var args = new Dictionary<string, object?>
        {
            ["tabParam"] = "",
            ["refFormId"] = "fa_football_bet_order",
            ["_t"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
        };

        var payload = new Dictionary<string, object?>
        {
            ["ordid"] = ordId,
            ["match_id"] = string.IsNullOrWhiteSpace(matchId) ? string.Empty : matchId
        };

        var headers = new Dictionary<string, string>
        {
            ["__userkeyid__"] = token,
            ["Origin"] = "https://site.vic88888.com",
            ["Referer"] = "https://site.vic88888.com/site/customize/fa_football_bet_order/view.do",
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/156.0.0.0 Safari/537.36 Edg/156.0.0.0"
        };

        return await PostAsync<GetOrderDetailResponse>(
            "site.vic88888.com",
            "/site/customize/fa_football_bet_order/executeService/get_json",
            args,
            payload,
            headers);
    }

}
