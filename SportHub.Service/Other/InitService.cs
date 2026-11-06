using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SportHub.Domain.Client.VIC;
using SportHub.Domain.Client.VIC.Response;
using SportHub.Domain.Other.Request;
using SportHub.Repository.Model;

using Zero.Core.Result;

namespace SportHub.Service.Other;

public class InitService(IServiceProvider provider) : BaseService(provider)
{
    private const long DefaultSourceCode = (long)global::SourceCode.VIC;

    private VICClient VicClient => _provider.GetService<VICClient>()!;

    public async Task<SysResult<bool>> SyncLeagueAsync(SyncLeagueRequest request)
    {
        var leagueListResult = await VicClient.GetLeagueListAsync(request.Token);
        if (leagueListResult.Code != ErrorCode.SYS_SUCCESS || leagueListResult.Result is null)
        {
            return Result(false, ErrorCode.SYS_FAIL, $"同步失败：VIC 请求失败。{leagueListResult.ErrorDesc}");
        }

        if (leagueListResult.Result.Code != 200)
        {
            return Result(false, ErrorCode.SYS_FAIL, $"同步失败：VIC 返回失败。{leagueListResult.Result.Msg}");
        }

        var leagues = BuildLeagueEntities(leagueListResult.Result.ListData);
        if (leagues.Count == 0)
        {
            return Result(false, ErrorCode.SYS_FAIL, "同步失败：未从数据源解析到任何联赛。");
        }

        await UpsertLeaguesAsync(leagues);
        await DB.SaveChangesAsync();

        return Result(true);
    }

    public async Task<SysResult<GetOrderDetailResponse>> GetOrderDetailAsync(GetOrderDetailRequest request)
    {
        var result = await VicClient.GetOrderDetailAsync(request.Token, request.OrderId);
        if (result.Code != ErrorCode.SYS_SUCCESS || result.Result is null)
        {
            return Result<GetOrderDetailResponse>(default, ErrorCode.SYS_FAIL, $"查询失败：VIC 请求失败。{result.ErrorDesc}");
        }

        if (result.Result.Code != 200)
        {
            return Result<GetOrderDetailResponse>(default, ErrorCode.SYS_FAIL, $"查询失败：VIC 返回失败。{result.Result.Msg}");
        }

        return Result(result.Result);
    }

    private static List<League> BuildLeagueEntities(List<ListDatum>? sourceItems)
    {
        if (sourceItems is null || sourceItems.Count == 0)
        {
            return [];
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return sourceItems
            .Where(item => item is not null)
            .Select(item => CreateLeagueEntity(item, now))
            .Where(item => item.Id > 0)
            .ToList();
    }

    private static League CreateLeagueEntity(ListDatum item, long now)
    {
        var sourceId = GetSourceId(item);

        return new League
        {
            Id = sourceId,
            NameEn = GetText(item.NameEn),
            NameZhCn = GetText(item.NameChs),
            NameZhTw = GetText(item.NameCht),
            ShortNameEn = GetText(item.NameEnShort),
            ShortNameZhCn = GetText(item.NameChsShort),
            ShortNameZhTw = GetText(item.NameChtShort),
            CountryId = item.CountryId > 0 ? item.CountryId : null,
            CountryEn = GetText(item.CountryEn),
            CountryZhCn = GetText(item.CountryCn),
            CountryZhTw = GetText(item.CountryCn),
            LeagueCode = NormalizeLeagueCodeFromType(item.Type),
            AliasArray = BuildAliasDictionary(DefaultSourceCode, sourceId),
            Flag = GetLong(item.IsDisabled, 1),
            CreatedTime = item.Createtime > 0 ? item.Createtime : now,
            UpdatedTime = item.Updatetime > 0 ? item.Updatetime : now
        };
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

    private static Dictionary<long, string>? BuildAliasDictionary(long sourceCode, long sourceId)
    {
        if (sourceCode <= 0 || sourceId <= 0)
        {
            return null;
        }

        return new Dictionary<long, string>
        {
            [sourceCode] = sourceId.ToString()
        };
    }

    private static long GetSourceId(ListDatum item)
    {
        if (item.Id > 0)
        {
            return item.Id;
        }

        if (item.LeagueId > 0)
        {
            return item.LeagueId;
        }

        return item.Keyid > 0 ? item.Keyid : 0;
    }

    private static string? GetText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static long GetLong(long value, long defaultValue)
    {
        return value > 0 ? value : defaultValue;
    }

    private static long NormalizeLeagueCodeFromType(long type)
    {
        return type switch
        {
            1 => (long)global::LeagueCode.LEAGUE,
            2 => (long)global::LeagueCode.CUP,
            _ => (long)global::LeagueCode.None
        };
    }
}