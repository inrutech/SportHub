using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Model;
using SportHub.Repository.Model;

using Microsoft.EntityFrameworkCore;

using Zero.Core.Extensions;

namespace SportHub.Service.Common
{
    public class TaskService(IServiceProvider provider) : BaseService(provider)
    {
        public Task<List<Repository.Model.Task>> CacheGetTaskListAsync()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "TaskList");
            return WebCache.Get(key, async () => await DB.Task.AsNoTracking().ToListAsync());
        }

        public void CacheRemoveTaskList()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "TaskList");
            WebCache.Remove(key);
        }

        public TaskInfo? RenderTaskInfo(Repository.Model.Task? info)
        {
            if (info == null) return null;

            return new TaskInfo
            {
                Id = info.Id,
                OrderId = info.OrderId,
                OrderLegIdList = info.OrderLegIdList,
                TargetSourceCode = (info.TargetSourceCode ?? 0).ToEnum(SourceCode.None),
                ProcessCode = (info.ProcessCode ?? 0).ToEnum(ProcessCode.None),
                Ratio = info.Ratio,
                CreatedTime = info.CreatedTime,
                UpdatedTime = info.UpdatedTime,
                Disabled = (info.Flag & (long)CommonCode.Disabled) > 0,
                Result = info.Result
            };
        }
    }
}
