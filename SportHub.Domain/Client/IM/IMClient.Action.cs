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
            ["cookie"] = $"Authorization={token}",
        };

        return await PostAsync<GetOrderDetailResponse>(
            "site.vic88888.com",
            "/site/customize/fa_football_bet_order/executeService/get_json",
            args,
            payload,
            headers);
    }

    public async Task<SysResult<GetOrderQueryResponse>> GetOrderQueryAsync(
        string apiKey,
        DateTime startTime,
        DateTime endTime,
        int settled,
        int page = 1,
        int pageSize = 100,
        string? orderId = null,
        string? userId = null,
        string? betType = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new SysResult<GetOrderQueryResponse>
            {
                Code = ErrorCode.SYS_FAIL,
                ErrorDesc = "缺少 API Key",
                Result = null
            };
        }

        var payload = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(orderId))
        {
            payload["orderid"] = orderId;
        }
        else
        {
            payload["start_time"] = startTime.ToString("yyyy-MM-dd HH:mm:ss");
            payload["end_time"] = endTime.ToString("yyyy-MM-dd HH:mm:ss");
            payload["settled"] = settled;
            payload["page"] = page <= 0 ? 1 : page;
            payload["page_size"] = pageSize <= 0 ? 100 : Math.Min(pageSize, 500);
        }

        if (!string.IsNullOrWhiteSpace(userId))
        {
            payload["user_id"] = userId;
        }

        if (!string.IsNullOrWhiteSpace(betType))
        {
            payload["bet_type"] = betType;
        }

        var headers = new Dictionary<string, string>
        {
            ["X-Api-Key"] = apiKey,
            ["Content-Type"] = "application/json"
        };

        return await PostAsync<GetOrderQueryResponse>("site.vic88888.com", "/openapi/order/query", null, payload, headers);
    }

}
