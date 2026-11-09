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
            ["cookie"] = $"Authorization={token}",
        };

        var r = await PostAsync<GetLeagueListResponse>("https://site.vic88888.com", "/site/customize/fa_football_league/executeService/listData", args, payload, headers);
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
            ["cookie"] = $"Authorization={token}",
        };

        return await PostAsync<GetOrderDetailResponse>(
            "https://site.vic88888.com",
            "/site/customize/fa_football_bet_order/executeService/get_json",
            args,
            payload,
            headers);
    }

    public async Task<SysResult<GetOrderQueryResponse>> GetOrderQueryAsync(
        string apiKey,
        DateTime startTime,
        DateTime endTime,
        string? orderId = null,
        int settled = 0)
    {


        var payload = new Dictionary<string, object?>()
        {
            ["start_time"] = startTime.ToString("yyyy-MM-dd HH:mm:ss"),
            ["end_time"] = endTime.ToString("yyyy-MM-dd HH:mm:ss"),
            ["settled"] = settled,
            ["orderid"] = orderId
        };

        var headers = new Dictionary<string, string>
        {
            ["X-Api-Key"] = apiKey,
            ["Content-Type"] = "application/json;charset=UTF-8"
        };

        return await PostAsync<GetOrderQueryResponse>("http://smartwork.ddns.net:26592", "/api/getOrders", null, payload, headers);
    }

}
