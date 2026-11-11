using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request.Team;
using SportHub.Domain.Admin.Response.Team;
using SportHub.Service.Admin;

using Zero.Core.Result;
using Zero.Core.Web;
using Zero.Core.Web.Request;

namespace SportHub.Controller.Admin;

public class TeamController(TeamService service) : BaseController
{
    [HttpGet("Page")]
    public Task<SysResult<PageList<TeamPageResponse>>> PageAsync([FromQuery] TeamPageRequest request)
    {
        return service.PageAsync(request);
    }
}
