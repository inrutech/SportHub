using Microsoft.EntityFrameworkCore;

using SportHub.Domain.Admin.Request.Team;
using SportHub.Domain.Admin.Response.Team;

using Zero.Core.Result;
using Zero.Core.Web;
using Zero.Core.Web.Request;

namespace SportHub.Service.Admin;

public class TeamService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<PageList<TeamPageResponse>>> PageAsync(TeamPageRequest request)
    {
        var query = DB.Team.AsNoTracking();

        if (request.Id > 0)
        {
            query = query.Where(x => x.Id == request.Id);
        }

        if (request.LeagueId > 0)
        {
            query = query.Where(x => x.LeagueId == request.LeagueId);
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

        var result = new PageList<TeamPageResponse>(
            list.Select(x => new TeamPageResponse
            {
                TeamInfo = base.TeamService.RenderTeamInfo(x),
            }).ToList(),
            request,
            count);

        return Result(result);
    }
}
