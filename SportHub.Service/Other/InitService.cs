using Microsoft.EntityFrameworkCore;

using SportHub.Repository.Model;

using System.Text.Json;
using System.Text.Json.Nodes;

using Zero.Core.Result;

namespace SportHub.Service.Other;

public class InitService(IServiceProvider provider) : BaseService(provider)
{
    private const string DefaultApiBaseUrl = "https://www.thesportsdb.com/api/v1/json";
    private const string DefaultApiKey = "123";

    public async Task<SysResult<bool>> SyncLeagueAsync()
    {
        var apiBaseUrl = GetEnvOrDefault("SPORTHUB_SYNC_API_BASEURL", DefaultApiBaseUrl).TrimEnd('/');
        var apiKey = GetEnvOrDefault("SPORTHUB_SYNC_API_KEY", DefaultApiKey);

        var leaguesUrl = $"{apiBaseUrl}/{apiKey}/all_leagues.php";

        var client = HttpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("SportHub/1.0");

        var leaguePayload = await client.GetStringAsync(leaguesUrl);

        var leagues = ParseLeagues(leaguePayload);
        if (leagues.Count == 0)
        {
            throw new InvalidOperationException("同步失败：未从数据源解析到任何联赛。请检查接口响应或字段映射。");
        }

        var teams = new List<Team>();

        foreach (var league in leagues)
        {
            if (string.IsNullOrWhiteSpace(league.Name))
            {
                continue;
            }

            var encodedLeagueName = Uri.EscapeDataString(league.Name);
            var teamsUrl = $"{apiBaseUrl}/{apiKey}/search_all_teams.php?l={encodedLeagueName}";

            string teamPayload;
            try
            {
                teamPayload = await client.GetStringAsync(teamsUrl);
            }
            catch
            {
                continue;
            }

            teams.AddRange(ParseTeams(teamPayload));
        }

        // The API can return duplicate teams across competitions; keep the latest payload by Id.
        teams = teams
            .GroupBy(x => x.Id)
            .Select(g => g.Last())
            .ToList();

        await UpsertLeaguesAsync(leagues);
        await UpsertTeamsAsync(teams);

        await DB.SaveChangesAsync();

        return Result(true);
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

            var id = GetInt64(item, "idLeague", "id", "Id", "leagueId", "league_id");
            if (id <= 0)
            {
                continue;
            }

            result.Add(new League
            {
                Id = id,
                Abbr = GetString(item, "strLeagueAlternate", "abbr", "Abbr", "short", "shortName", "code", "Code"),
                Name = GetString(item, "strLeague", "name", "Name"),
                CountryCode = GetString(item, "strCountryCode", "countryCode", "country_code", "countryIso", "country_iso", "fifaCode", "fifa_code"),
                LeagueCode = NormalizeLeagueCode(GetString(item, "strSport", "leagueCode", "LeagueCode", "leagueType", "LeagueType", "type", "Type", "kind", "Kind", "category", "Category")),
                Country = GetString(item, "strCountry", "country", "Country"),
                Region = GetString(item, "region", "Region", "continent", "confederation"),
                AliasArray = BuildAliasDictionary(
                    [
                        ("thesportsdb", GetString(item, "idLeague")),
                        ("espn", GetString(item, "idESPN")),
                        ("apifootball", GetString(item, "idAPIfootball"))
                    ],
                    [GetString(item, "strLeagueAlternate"), GetString(item, "strLeague")]),
                MetaJson = GetObjectJson(item, "meta", "metadata", "extra"),
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

            var id = GetInt64(item, "idTeam", "id", "Id", "teamId", "team_id");
            if (id <= 0)
            {
                continue;
            }

            result.Add(new Team
            {
                Id = id,
                Abbr = GetString(item, "strTeamShort", "abbr", "Abbr", "short", "shortName", "code", "Code"),
                Name = GetString(item, "strTeam", "name", "Name"),
                ShortName = GetString(item, "strTeamShort", "shortName", "ShortName", "abbr", "Abbr", "short", "Short"),
                EnglishName = GetString(item, "englishName", "EnglishName", "enName", "en_name"),
                CountryCode = GetString(item, "countryCode", "country_code", "countryIso", "country_iso", "fifaCode", "fifa_code"),
                Country = GetString(item, "strCountry", "country", "Country"),
                City = GetString(item, "strLocation", "city", "City", "location", "Location"),
                FoundedYear = GetNullableInt64(item, "intFormedYear", "foundedYear", "FoundedYear", "founded", "Founded"),
                Stadium = GetString(item, "strStadium", "stadium", "Stadium", "ground", "Ground"),
                AliasArray = BuildAliasDictionary(
                    [
                        ("thesportsdb", GetString(item, "idTeam")),
                        ("espn", GetString(item, "idESPN")),
                        ("apifootball", GetString(item, "idAPIfootball"))
                    ],
                    [GetString(item, "strTeamAlternate"), GetString(item, "strTeam")]),
                MetaJson = GetObjectJson(item, "meta", "metadata", "extra"),
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
                target.Abbr = source.Abbr;
                target.CountryCode = source.CountryCode;
                target.LeagueCode = source.LeagueCode;
                target.Country = source.Country;
                target.Region = source.Region;
                target.AliasArray = source.AliasArray;
                target.MetaJson = source.MetaJson;
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
                target.Abbr = source.Abbr;
                target.ShortName = source.ShortName;
                target.EnglishName = source.EnglishName;
                target.CountryCode = source.CountryCode;
                target.Country = source.Country;
                target.City = source.City;
                target.FoundedYear = source.FoundedYear;
                target.Stadium = source.Stadium;
                target.AliasArray = source.AliasArray;
                target.MetaJson = source.MetaJson;
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

    private static string BuildAliasDictionary(
        IEnumerable<(string platform, string? key)> keys,
        IEnumerable<string?> aliases)
    {
        var keyObj = new JsonObject();
        foreach (var (platform, key) in keys)
        {
            if (string.IsNullOrWhiteSpace(platform) || string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            keyObj[platform] = key;
        }

        var aliasArray = new JsonArray();
        foreach (var alias in aliases.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!.Trim()).Distinct())
        {
            aliasArray.Add(alias);
        }

        var root = new JsonObject
        {
            ["keys"] = keyObj,
            ["aliases"] = aliasArray
        };

        return root.ToJsonString();
    }

    private static string? GetObjectJson(JsonObject source, params string[] keys)
    {
        foreach (var key in keys)
        {
            var node = source[key];
            if (node is JsonObject or JsonArray)
            {
                return node.ToJsonString();
            }
        }

        return null;
    }

    private static long NormalizeLeagueCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return (long)global::LeagueCode.None;
        }

        var normalized = value.Trim().ToUpperInvariant();
        return normalized switch
        {
            "SOCCER" => (long)global::LeagueCode.LEAGUE,
            "LEAGUE" or "LIGA" or "DIVISION" => (long)global::LeagueCode.LEAGUE,
            "CUP" => (long)global::LeagueCode.CUP,
            "SUPER" or "SUPERCUP" => (long)global::LeagueCode.SUPER,
            "INTERNATIONAL" => (long)global::LeagueCode.INTERNATIONAL,
            _ => (long)global::LeagueCode.None
        };
    }
}