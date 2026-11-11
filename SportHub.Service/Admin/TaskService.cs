
using SportHub.Domain.Admin.Request.Task;
using SportHub.Domain.Admin.Response.Task;

using Zero.Core.Result;

namespace SportHub.Service.Admin;

public class TaskService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<List<TaskPageResponse>>> PageAsync(TaskPageRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<SysResult<bool>> SetAsync(SetRequest request)
    {
        throw new NotImplementedException();
    }
}