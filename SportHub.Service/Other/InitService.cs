using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SportHub.Domain.Client.VIC;
using SportHub.Domain.Client.VIC.Request;
using SportHub.Repository.Model;

using System.Text.Json.Nodes;

using Zero.Core.Result;

namespace SportHub.Service.Other;

public class InitService(IServiceProvider provider) : BaseService(provider)
{
    private const long DefaultSourceCode = (long)global::SourceCode.VIC;
    private VICClient VicClient => _provider.GetService<VICClient>()!;

    public async Task<SysResult<bool>> SyncLeagueAsync(SyncLeagueRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Token))
        {
            throw new InvalidOperationException("同步失败：请求缺少 Token。请在 header 映射字段 __userkeyid__ 对应的 Token 传入请求体。\n");
        }

        var payloadResult = await VicClient.GetLeagueListPayloadAsync(new VicLeagueListRequest
        {
            Token = request.Token
        });

        if (payloadResult.Code != ErrorCode.SYS_SUCCESS || string.IsNullOrWhiteSpace(payloadResult.Result))
        {
            throw new InvalidOperationException($"同步失败：VIC 请求失败。{payloadResult.ErrorDesc}");
        }

        var leagues = ParseLeaguesFromVic(payloadResult.Result);
        if (leagues.Count == 0)
        {
            throw new InvalidOperationException("同步失败：未从数据源解析到任何联赛。请检查接口响应或字段映射。");
        }

        await UpsertLeaguesAsync(leagues);

        await DB.SaveChangesAsync();

        return Result(true);
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