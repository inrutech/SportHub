
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
        if (!long.TryParse(request.OrderId, out var orderId) || orderId <= 0)
        {
            return Result(new List<TaskPageResponse>(), ErrorCode.USER_CUSTOM, "OrderId invalid");
        }

        var list = await DB.Task.AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.CreatedTime)
            .ToListAsync();

        var order = await DB.Order.AsNoTracking().FirstOrDefaultAsync(x => x.Id == orderId);
        var legList = await DB.OrderLeg.AsNoTracking().Where(x => x.OrderId == orderId).ToListAsync();
        var leagueList = await DB.League.AsNoTracking().ToListAsync();
        var teamList = await DB.Team.AsNoTracking().ToListAsync();
        var matchList = await DB.Match.AsNoTracking().ToListAsync();

        var result = list.Select(x => new TaskPageResponse
        {
            TaskInfo = TaskService.RenderTaskInfo(x),
            OrderInfo = OrderService.RenderOrderInfo(order),
            OrderLegList = legList.Select(OrderLegService.RenderOrderLegInfo).ToList(),
            LeagueList = leagueList.Select(LeagueService.RenderLeagueInfo).ToList(),
            TeamList = teamList.Select(TeamService.RenderTeamInfo).ToList(),
            MatchList = matchList.Select(MatchService.RenderMatchInfo).ToList(),

        }).ToList();

        return Result(result);
    }

    public async Task<SysResult<bool>> SetAsync(SetRequest request)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

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