using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request.League;
using SportHub.Domain.Admin.Response.League;
using SportHub.Service.Admin;

using Zero.Core.Result;
using Zero.Core.Web;
using Zero.Core.Web.Request;

namespace SportHub.Controller.Admin;

public class LeagueController(LeagueService service) : BaseController
{

    [HttpGet("Page")]
    public Task<SysResult<PageList<LeaguePageResponse>>> PageAsync([FromQuery] LeaguePageRequest request)
    {
        return service.PageAsync(request);
    }

    [HttpPost("SyncLeague")]
    public Task<SysResult<bool>> SyncLeagueAsync()
    {
        return service.SyncLeagueAsync();
    }

}