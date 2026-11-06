using Microsoft.Extensions.Logging;

using SportHub.Domain.Client.VIC.Request;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Zero.Core.Result;

namespace SportHub.Domain.Client.VIC;

public partial class VICClient
{
    public async Task<SysResult<string>> GetLeagueListPayloadAsync(VicLeagueListRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Fail<string>("请求缺少 Token");
        }

        var url = $"https://site.vic88888.com/site/customize/fa_football_league/executeService/listData?tabParam=&refFormId=fa_football_league&_t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var leaguesUrl = GetEnvOrDefault("SPORTHUB_SYNC_LEAGUE_API_URL", url);

        var ignoreSslErrors = string.Equals(
            GetEnvOrDefault("SPORTHUB_SYNC_IGNORE_SSL_ERRORS", "false"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        var useProxy = string.Equals(
            GetEnvOrDefault("SPORTHUB_SYNC_USE_PROXY", "true"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        using var client = CreateHttpClient(ignoreSslErrors, useProxy);
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(DefaultLeagueUserAgent);
        client.DefaultRequestHeaders.Remove("__userkeyid__");
        client.DefaultRequestHeaders.Add("__userkeyid__", request.Token);

        var payload = new
        {
            tabParam = "",
            filter = "",
            _pageParam_ = "eyJfb2Zmc2V0XyI6MSwiX2xpbWl0XyI6NTAwMCwiX29yZGVyQnlfIjoiY3JlYXRldGltZV9uYW1lIGRlc2MiLCJfY2FsY0ZpZWxkXyI6ImNvdW50KGlkKSJ9"
        };

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, leaguesUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=UTF-8");
        requestMessage.Headers.Accept.Clear();
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Headers.Remove("Cookie");
        requestMessage.Headers.Add("Cookie", string.IsNullOrWhiteSpace(request.Cookie) ? DefaultLeagueCookie : request.Cookie);
        requestMessage.Headers.Referrer = new Uri(DefaultLeagueReferer);
        requestMessage.Version = HttpVersion.Version11;
        requestMessage.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

        try
        {
            using var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return new SysResult<string>
            {
                Code = ErrorCode.SYS_SUCCESS,
                Result = content
            };
        }
        catch (HttpRequestException ex)
        {
            var detail = FlattenExceptionMessages(ex);
            Logger?.LogError(ex, "VIC 请求失败: {Detail}", detail);
            return Fail<string>($"HTTPS 请求异常，目标地址 {leaguesUrl}。详细信息：{detail}");
        }
        catch (Exception ex)
        {
            var detail = FlattenExceptionMessages(ex);
            Logger?.LogError(ex, "VIC 请求异常: {Detail}", detail);
            return Fail<string>($"请求异常：{detail}");
        }
    }

    public Task<SysResult<SendSMSResponse>> SendSMSAsync(string? mobile, string? code, string? creds)
    {
        string url = RenderUrl("api.laaffic.com", "v3/sendSms");

        var data = new Dictionary<string, object?>
        {
            { "appId", AppId },
            { "numbers", $"91{mobile}" },
            { "content", $"Your verification code is: {code}. For your security, this code expires in 3 minutes. Please do not share this code with anyone." },
            { "senderId", "UPIBee" },
            { "orderId", creds }
        };

        return PostAsync<SendSMSResponse>(url, data);
    }
}
