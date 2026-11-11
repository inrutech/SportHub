
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

        var result = list.Select(x => new TaskPageResponse
        {
            TaskInfo = TaskService.RenderTaskInfo(x),
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
            TargetSourceCode = request.TargetSourceCode,
            Flag = 0,
            CreatedTime = now,
            UpdatedTime = now,
        };

        await DB.Task.AddAsync(task);
        await DB.SaveChangesAsync();
        return Result(true);
    }

}