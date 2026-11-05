using Microsoft.EntityFrameworkCore;

using SportHub.Domain.Other.Request;
using SportHub.Repository.Model;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using Zero.Core.Result;

namespace SportHub.Service.Other;

public class InitService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<bool>> SyncLeagueAsync(SyncLeagueRequest request)
    {
        var leaguePayload = await GetSourcePayloadAsync(request, request.LeagueUrl, request.LeaguePath);
        var teamPayload = await GetSourcePayloadAsync(request, request.TeamUrl, request.TeamPath);

        var leagues = ParseLeagues(leaguePayload);
        var teams = ParseTeams(teamPayload);

        await UpsertLeaguesAsync(leagues);
        await UpsertTeamsAsync(teams);

        await DB.SaveChangesAsync();

        return Result(true);
    }

    private async Task<string> GetSourcePayloadAsync(
        SyncLeagueRequest request,
        string? directUrl,
        string fallbackPath)
    {
        var client = HttpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("SportHub/1.0");

        if (!string.IsNullOrWhiteSpace(directUrl))
        {
            return await client.GetStringAsync(directUrl);
        }

        if (string.IsNullOrWhiteSpace(request.Owner) || string.IsNullOrWhiteSpace(request.Repo))
        {
            throw new InvalidOperationException("未提供 LeagueUrl/TeamUrl 时，必须提供 Owner 和 Repo。");
        }

        var escapedPath = Uri.EscapeDataString(fallbackPath).Replace("%2F", "/");
        var url =
            $"https://api.github.com/repos/{request.Owner}/{request.Repo}/contents/{escapedPath}?ref={request.Branch}";

        using var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        if (!string.IsNullOrWhiteSpace(request.Token))
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);
        }

        using var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();
        var jsonObject = JsonNode.Parse(responseText)?.AsObject();
        var content = jsonObject?["content"]?.GetValue<string>();
        if (!string.IsNullOrWhiteSpace(content))
        {
            var base64Text = content.Replace("\r", string.Empty).Replace("\n", string.Empty);
            var bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(bytes);
        }

        var downloadUrl = jsonObject?["download_url"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(downloadUrl))
        {
            throw new InvalidOperationException("未能从 GitHub 返回中解析文件内容。");
        }

        return await client.GetStringAsync(downloadUrl);
    }

    private static List<League> ParseLeagues(string payload)
    {
        var array = ExtractDataArray(payload);
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var result = new List<League>(array.Count);

        foreach (var node in array)
        {
            if (node is not JsonObject item)
            {
                continue;
            }

            var id = GetInt64(item, "id", "Id", "leagueId", "league_id");
            if (id <= 0)
            {
                continue;
            }

            result.Add(new League
            {
                Id = id,
                Name = GetString(item, "name", "Name"),
                Country = GetString(item, "country", "Country"),
                AliasArray = GetAliasArray(item, "aliasArray", "AliasArray", "aliases", "alias"),
                Flag = GetInt64(item, ["flag", "Flag", "status", "Status"], 1),
                CreatedTime = GetInt64(item, ["createdTime", "CreatedTime", "created_at"], now),
                UpdatedTime = GetInt64(item, ["updatedTime", "UpdatedTime", "updated_at"], now)
            });
        }

        return result;
    }

    private static List<Team> ParseTeams(string payload)
    {
        var array = ExtractDataArray(payload);
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var result = new List<Team>(array.Count);

        foreach (var node in array)
        {
            if (node is not JsonObject item)
            {
                continue;
            }

            var id = GetInt64(item, "id", "Id", "teamId", "team_id");
            if (id <= 0)
            {
                continue;
            }

            result.Add(new Team
            {
                Id = id,
                Name = GetString(item, "name", "Name"),
                Country = GetString(item, "country", "Country"),
                AliasArray = GetAliasArray(item, "aliasArray", "AliasArray", "aliases", "alias"),
                Flag = GetInt64(item, ["flag", "Flag", "status", "Status"], 1),
                CreatedTime = GetInt64(item, ["createdTime", "CreatedTime", "created_at"], now),
                UpdatedTime = GetInt64(item, ["updatedTime", "UpdatedTime", "updated_at"], now)
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
                target.Name = source.Name;
                target.Country = source.Country;
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

    private async Task UpsertTeamsAsync(List<Team> teams)
    {
        if (teams.Count == 0)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var ids = teams.Select(x => x.Id).Distinct().ToList();
        var existing = await DB.Team
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        foreach (var source in teams)
        {
            if (existing.TryGetValue(source.Id, out var target))
            {
                target.Name = source.Name;
                target.Country = source.Country;
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

            await DB.Team.AddAsync(source);
        }
    }

    private static JsonArray ExtractDataArray(string payload)
    {
        var node = JsonNode.Parse(payload);
        if (node is JsonArray rootArray)
        {
            return rootArray;
        }

        if (node is JsonObject rootObject)
        {
            var candidates = new[] { "data", "items", "list", "leagues", "teams", "result" };
            foreach (var name in candidates)
            {
                if (rootObject[name] is JsonArray dataArray)
                {
                    return dataArray;
                }
            }
        }

        throw new InvalidOperationException("数据格式不正确，未找到数组节点。");
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

    private static string? GetAliasArray(JsonObject source, params string[] keys)
    {
        foreach (var key in keys)
        {
            var node = source[key];
            if (node is null)
            {
                continue;
            }

            if (node is JsonArray array)
            {
                return array.ToJsonString();
            }

            if (node is JsonValue value && value.TryGetValue<string>(out var text))
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                var trimmed = text.Trim();
                if ((trimmed.StartsWith("[") && trimmed.EndsWith("]")) ||
                    (trimmed.StartsWith("{") && trimmed.EndsWith("}")))
                {
                    return trimmed;
                }

                return JsonSerializer.Serialize(new[] { trimmed });
            }

            return node.ToJsonString();
        }

        return null;
    }
}