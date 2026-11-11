using Microsoft.EntityFrameworkCore;

using SportHub.Domain.Admin.Request.Match;
using SportHub.Domain.Admin.Response.Match;

using Zero.Core.Result;
using Zero.Core.Web;
using Zero.Core.Web.Request;

namespace SportHub.Service.Admin;

public class MatchService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<PageList<MatchPageResponse>>> PageAsync(MatchPageRequest request)
    {
        var query = DB.Match.AsNoTracking();

        if (request.Id > 0)
        {
            query = query.Where(x => x.Id == request.Id);
        }

        if (request.LeagueId > 0)
        {
            query = query.Where(x => x.LeagueId == request.LeagueId);
        }

        if (request.HomeTeamId > 0)
        {
            query = query.Where(x => x.HomeTeamId == request.HomeTeamId);
        }

        if (request.AwayTeamId > 0)
        {
            query = query.Where(x => x.AwayTeamId == request.AwayTeamId);
        }

        var list = await query.OrderByDescending(x => x.CreatedTime)
            .Skip(request.PageSkip)
            .Take(request.PageSize)
            .ToListAsync();
        var count = await query.CountAsync();

        var result = new PageList<MatchPageResponse>(
            list.Select(x => new MatchPageResponse
            {
                MatchInfo = base.MatchService.RenderMatchInfo(x),
            }).ToList(),
            request,
            count);

        return Result(result);
    }
}
