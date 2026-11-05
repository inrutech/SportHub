using Microsoft.EntityFrameworkCore;

using SportHub.Repository.Model;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using Zero.Core.Result;

namespace SportHub.Service.Other;

public class InitService(IServiceProvider provider) : BaseService(provider)
{
    private const long DefaultSourceCode = (long)global::SourceCode.VIC;
    private const string DefaultLeagueReferer = "https://site.vic88888.com/site/customize/fa_football_league/view.do";
    private const string DefaultLeagueUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/156.0.0.0 Safari/537.36 Edg/156.0.0.0";
    private const string DefaultLeagueCookie = "Authorization=eca4f808-f86e-4838-9fd5-a368eed400e3";
    private const string DefaultProxyUrl = "http://127.0.0.1:7890";

    public async Task<SysResult<bool>> SyncLeagueAsync(SyncLeagueRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Token))
        {
            throw new InvalidOperationException("同步失败：请求缺少 Token。请在 header 映射字段 __userkeyid__ 对应的 Token 传入请求体。\n");
        }

        var url = $"https://site.vic88888.com/site/customize/fa_football_league/executeService/listData?tabParam=&refFormId=fa_football_league&_t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var leaguesUrl = GetEnvOrDefault("SPORTHUB_SYNC_LEAGUE_API_URL", url);

        var ignoreSslErrors = string.Equals(
            GetEnvOrDefault("SPORTHUB_SYNC_IGNORE_SSL_ERRORS", "false"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        using var client = CreateHttpClient(ignoreSslErrors);
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
        requestMessage.Headers.Add("Cookie", DefaultLeagueCookie);
        requestMessage.Headers.Referrer = new Uri(DefaultLeagueReferer);
        requestMessage.Version = HttpVersion.Version11;
        requestMessage.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var detail = FlattenExceptionMessages(ex);
            throw new InvalidOperationException(
                $"同步失败：HTTPS 请求异常，目标地址 {leaguesUrl}。详细信息：{detail}",
                ex);
        }

        using (response)
        {
            var leaguePayload = await response.Content.ReadAsStringAsync();

            var leagues = ParseLeaguesFromVic(leaguePayload);
            if (leagues.Count == 0)
            {
                throw new InvalidOperationException("同步失败：未从数据源解析到任何联赛。请检查接口响应或字段映射。");
            }

            await UpsertLeaguesAsync(leagues);

            await DB.SaveChangesAsync();
        }

        return Result(true);
    }

    private HttpClient CreateHttpClient(bool ignoreSslErrors)
    {
        var useProxy = string.Equals(
            GetEnvOrDefault("SPORTHUB_SYNC_USE_PROXY", "true"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (!ignoreSslErrors && !useProxy)
        {
            return HttpClientFactory.CreateClient();
        }

        var handler = new HttpClientHandler();

        if (ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        }

        if (useProxy)
        {
            var proxyUrl = GetEnvOrDefault("SPORTHUB_SYNC_PROXY", DefaultProxyUrl);
            var proxy = new WebProxy(proxyUrl)
            {
                BypassProxyOnLocal = false
            };

            handler.Proxy = proxy;
            handler.UseProxy = true;
        }

        return new HttpClient(handler, disposeHandler: true);
    }

    private static string GetEnvOrDefault(string key, string fallback)
    {
        var envValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(envValue))
        {
            return envValue;
        }

        return fallback;
    }

    private static string FlattenExceptionMessages(Exception ex)
    {
        var messages = new List<string>();
        Exception? current = ex;

        while (current is not null)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
            {
                messages.Add(current.Message.Trim());
            }

            current = current.InnerException;
        }

        return string.Join(" | ", messages.Distinct());
    }

    private static List<League> ParseLeaguesFromVic(string payload)
    {
        var array = ExtractVicListData(payload);
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var result = new List<League>(array.Count);

        foreach (var node in array)
        {
            if (node is not JsonObject item)
            {
                continue;
            }

            var id = GetInt64(item, "id", "keyid", "Id", "league_id", "leagueId");
            if (id <= 0)
            {
                continue;
            }

            result.Add(new League
            {
                Id = id,
                NameEn = GetString(item, "name_en"),
                NameZhCn = GetString(item, "name_chs"),
                NameZhTw = GetString(item, "name_cht"),
                ShortNameEn = GetString(item, "name_en_short"),
                ShortNameZhCn = GetString(item, "name_chs_short"),
                ShortNameZhTw = GetString(item, "name_cht_short"),
                CountryId = GetNullableInt64(item, "country_id"),
                CountryEn = GetString(item, "country_en"),
                CountryZhCn = GetString(item, "country_cn"),
                CountryZhTw = GetString(item, "country_cht", "country_cn"),
                LeagueCode = NormalizeLeagueCodeFromType(GetString(item, "type")),
                AliasArray = BuildAliasDictionary(DefaultSourceCode, GetString(item, "keyid", "id")),
                Flag = GetInt64(item, ["flag", "Flag", "status", "Status"], 1),
                CreatedTime = GetInt64(item, ["createtime", "createdTime", "CreatedTime", "created_at"], now),
                UpdatedTime = GetInt64(item, ["updatetime", "updatedTime", "UpdatedTime", "updated_at"], now)
            });
        }

        return result;
    }

    private async Task UpsertLeaguesAsync(List<League> leagues)
    {
        if (leagues.Count == 0)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var ids = leagues.Select(x => x.Id).Distinct().ToList();
        var existing = await DB.League
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        foreach (var source in leagues)
        {
            if (existing.TryGetValue(source.Id, out var target))
            {
                target.NameEn = source.NameEn;
                target.NameZhCn = source.NameZhCn;
                target.NameZhTw = source.NameZhTw;
                target.ShortNameEn = source.ShortNameEn;
                target.ShortNameZhCn = source.ShortNameZhCn;
                target.ShortNameZhTw = source.ShortNameZhTw;
                target.CountryId = source.CountryId;
                target.CountryEn = source.CountryEn;
                target.CountryZhCn = source.CountryZhCn;
                target.CountryZhTw = source.CountryZhTw;
                target.LeagueCode = source.LeagueCode;
                target.AliasArray = source.AliasArray;
                target.Flag = source.Flag;
                target.UpdatedTime = now;
                continue;
            }

            if (source.CreatedTime <= 0)
            {
                source.CreatedTime = now;
            }

            if (source.UpdatedTime <= 0)
            {
                source.UpdatedTime = now;
            }

            await DB.League.AddAsync(source);
        }
    }

    private static JsonArray ExtractVicListData(string payload)
    {
        var node = JsonNode.Parse(payload);

        if (node is JsonObject rootObject)
        {
            var code = GetInt64(rootObject, ["code"], 0);
            if (code != 200)
            {
                throw new InvalidOperationException($"同步失败：VIC 接口返回 code={code}。\n");
            }

            if (rootObject["listData"] is JsonArray dataArray)
            {
                return dataArray;
            }
        }

        throw new InvalidOperationException("同步失败：VIC 返回格式不正确，未找到 listData 数组。\n");
    }

    private static string? GetString(JsonObject source, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (source[key] is not JsonValue value)
            {
                continue;
            }

            if (value.TryGetValue<string>(out var text))
            {
                return text;
            }

            return value.ToString();
        }

        return null;
    }

    private static long GetInt64(JsonObject source, params string[] keys)
    {
        return GetInt64(source, keys, 0);
    }

    private static long GetInt64(JsonObject source, string[] keys, long defaultValue)
    {
        foreach (var key in keys)
        {
            if (source[key] is not JsonValue value)
            {
                continue;
            }

            if (value.TryGetValue<long>(out var longValue))
            {
                return longValue;
            }

            if (value.TryGetValue<int>(out var intValue))
            {
                return intValue;
            }

            if (long.TryParse(value.ToString(), out var parsedValue))
            {
                return parsedValue;
            }
        }

        return defaultValue;
    }

    private static long? GetNullableInt64(JsonObject source, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (source[key] is not JsonValue value)
            {
                continue;
            }

            if (value.TryGetValue<long>(out var longValue))
            {
                return longValue;
            }

            if (value.TryGetValue<int>(out var intValue))
            {
                return intValue;
            }

            if (long.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }
        }

        return null;
    }

    private static Dictionary<long, string>? BuildAliasDictionary(long sourceCode, string? keyId)
    {
        if (sourceCode <= 0 || string.IsNullOrWhiteSpace(keyId))
        {
            return null;
        }

        return new Dictionary<long, string>
        {
            [sourceCode] = keyId.Trim()
        };
    }

    private static long NormalizeLeagueCodeFromType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return (long)global::LeagueCode.None;
        }

        var normalized = value.Trim();
        return normalized switch
        {
            "1" => (long)global::LeagueCode.LEAGUE,
            "2" => (long)global::LeagueCode.CUP,
            _ => (long)global::LeagueCode.None
        };
    }
}