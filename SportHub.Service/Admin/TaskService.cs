
using Microsoft.EntityFrameworkCore;

using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Admin.Request.Task;
using SportHub.Domain.Admin.Response.Task;
using SportHub.Domain.Model;

using Zero.Core.Extensions;
using Zero.Core.Inject;
using Zero.Core.Result;

namespace SportHub.Service.Admin;

public class TaskService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<List<TaskPageResponse>>> PageAsync(TaskPageRequest request)
    {
        var query = DB.Task.AsNoTracking().AsQueryable();
        if (request.OrderId.HasValue)
        {
            query = query.Where(x => x.OrderId == request.OrderId);
        }
        var list = await query
            .OrderByDescending(x => x.CreatedTime)
            .ToListAsync();
        var orderIdList = list.Select(x => x.OrderId).Distinct().ToList();
        var orderLegIdList = list.SelectMany(x => x.OrderLegIdList).Distinct().ToList();
        var orderList = await DB.Order.Where(x => orderIdList.Contains(x.Id)).ToListAsync();
        var legList = await DB.OrderLeg.AsNoTracking().Where(x => orderLegIdList.Contains(x.Id)).ToListAsync();

        var leagueIdList = legList.Select(x => x.LeagueId).Distinct().ToList();
        var teamIdList = legList.SelectMany(x => new long?[] { x.HomeTeamId, x.AwayTeamId }).Distinct().ToList();
        var matchIdList = legList.Select(x => x.MatchId).Distinct().ToList();

        var leagueList = await DB.League.Where(x => leagueIdList.Contains(x.Id)).ToListAsync();
        var teamList = await DB.Team.Where(x => teamIdList.Contains(x.Id)).ToListAsync();
        var matchList = await DB.Match.Where(x => matchIdList.Contains(x.Id)).ToListAsync();

        var result = list.Select(x => new TaskPageResponse
        {
            TaskInfo = TaskService.RenderTaskInfo(x),
            OrderInfo = OrderService.RenderOrderInfo(orderList.FirstOrDefault(o => o.Id == x.OrderId)),
            OrderLegList = legList.Where(l => x.OrderLegIdList.Contains(l.Id)).Select(OrderLegService.RenderOrderLegInfo).ToList(),
            LeagueList = leagueList.Select(LeagueService.RenderLeagueInfo).ToList(),
            TeamList = teamList.Select(TeamService.RenderTeamInfo).ToList(),
            MatchList = matchList.Select(MatchService.RenderMatchInfo).ToList(),

        }).ToList();

        return Result(result);
    }

    public async Task<SysResult<bool>> SetAsync(SetRequest request)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var task = new Repository.Model.Task
        {
            Id = Snowflake.NewID(),
            OrderId = request.OrderId,
            OrderLegIdList = request.OrderLegIdList,
            TargetSourceCode = (long)request.TargetSourceCode,
            ProcessCode = (long)ProcessCode.Queued,
            Flag = 0,
            CreatedTime = now,
            UpdatedTime = now,
        };

        await DB.Task.AddAsync(task);
        await DB.SaveChangesAsync();
        return Result(true);
    }

}