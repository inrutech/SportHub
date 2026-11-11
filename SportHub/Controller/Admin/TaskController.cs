using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request.Task;
using SportHub.Domain.Admin.Response.Task;
using SportHub.Service.Admin;

using Zero.Core.Result;
using Zero.Core.Web.Request;

namespace SportHub.Controller.Admin;

public class TaskController(TaskService service) : BaseController
{

    [HttpPost("Set")]
    public Task<SysResult<bool>> SetAsync(SetRequest request)
    {
        return service.SetAsync(request);
    }

    [HttpGet("List")]
    public Task<SysResult<List<TaskPageResponse>>> PageAsync([FromQuery] TaskPageRequest request)
    {
        return service.PageAsync(request);
    }
}