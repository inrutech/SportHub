using Microsoft.EntityFrameworkCore;

using SportHub.Core;
using SportHub.Domain.Admin.Request.League;
using SportHub.Domain.Admin.Response.League;
using SportHub.Repository.Model;

using Zero.Core.Result;
using Zero.Core.Web;
using Zero.Core.Web.Request;

namespace SportHub.Service.Admin;

public class LeagueService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<PageList<LeaguePageResponse>>> PageAsync(LeaguePageRequest request)
    {
        var query = DB.League.AsNoTracking();

        if (request.Id > 0)
        {
            query = query.Where(x => x.Id == request.Id);
        }

        if (!string.IsNullOrWhiteSpace(request.NameEn))
        {
            query = query.Where(x => x.NameEn != null && x.NameEn.Contains(request.NameEn));
        }

        if (!string.IsNullOrWhiteSpace(request.NameZhCn))
        {
            query = query.Where(x => x.NameZhCn != null && x.NameZhCn.Contains(request.NameZhCn));
        }

        var list = await query.OrderByDescending(x => x.CreatedTime)
            .Skip(request.PageSkip)
            .Take(request.PageSize)
            .ToListAsync();
        var count = await query.CountAsync();

        var result = new PageList<LeaguePageResponse>(
            list.Select(x => new LeaguePageResponse
            {
                LeagueInfo = base.LeagueService.RenderLeagueInfo(x),
            }).ToList(),
            request,
            count);

        return Result(result);
    }

    public async Task<SysResult<bool>> SyncLeagueAsync()
    {
        var token = await ConfigService.GetValueAsync(GlobalHash.LoginToken);
        var leagueListResult = await IMClient.GetLeagueListAsync(token);
        if (leagueListResult.OccurError || leagueListResult.Result?.Code != 200)
        {
            return Result(false, ErrorCode.SYS_FAIL, $"同步失败：IM 请求失败。{leagueListResult.ErrorDesc}");
        }

        var leagueList = leagueListResult.Result.ListData.Select(x => new League
        {
            Id = x.Id,
            NameEn = x.NameEn,
            NameZhCn = x.NameChs,
            NameZhTw = x.NameCht,
            ShortNameEn = x.NameEnShort,
            ShortNameZhCn = x.NameChsShort,
            ShortNameZhTw = x.NameChtShort,
            CountryId = x.CountryId,
            CountryEn = x.CountryEn,
            CountryZhCn = x.CountryCn,
            CountryZhTw = x.CountryCn,
            LeagueCode = x.Type switch
            {
                1 => (long)LeagueCode.LEAGUE,
                2 => (long)LeagueCode.CUP,
                _ => (long)LeagueCode.None
            },
            SourceArray = new Dictionary<long, string>
            {
                [(long)SourceCode.IM] = x.Id.ToString()
            }
        }).ToList();
        var leagueOldList = await DB.League.ToListAsync();
        foreach (var item in leagueList)
        {
            var league = leagueOldList.FirstOrDefault(x => x.Id == item.Id);
            if (league == null)
            {
                league = new League
                {
                    Id = item.Id,
                    Flag = 0,
                    CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                await DB.League.AddAsync(league);
            }
            league.NameEn = item.NameEn;
            league.NameZhCn = item.NameZhCn;
            league.NameZhTw = item.NameZhTw;
            league.ShortNameEn = item.ShortNameEn;
            league.ShortNameZhCn = item.ShortNameZhCn;
            league.ShortNameZhTw = item.ShortNameZhTw;
            league.CountryId = item.CountryId;
            league.CountryEn = item.CountryEn;
            league.CountryZhCn = item.CountryZhCn;
            league.CountryZhTw = item.CountryZhTw;
            league.LeagueCode = item.LeagueCode;
            league.SourceArray = item.SourceArray;
            league.UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        await DB.SaveChangesAsync();
        return Result(true);
    }
}